using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Application.Contracts.Import;
using Topsis.Domain;
using Topsis.Domain.Contracts;

namespace Topsis.Adapters.Import
{
    public class ImportService : IImportService
    {
        private readonly IWorkspaceRepository _workspaces;
        private readonly IVoteRepository _votes;
        private readonly IApplicationUserRepository<ApplicationUser> _users;
        private readonly IUserContext _user;
        private Dictionary<string, string> _countriesMap;
        private readonly Country[] _allCountries = Country.AllCountries();
        private Dictionary<string, int> _workMap;
        private readonly JobCategory[] _allJobCategories = JobCategory.AllJobCategories();

        public ImportService(IWorkspaceRepository workspaces, 
            IVoteRepository votes,
            IApplicationUserRepository<ApplicationUser> users,
            IUserContext user)
        {
            _workspaces = workspaces;
            _votes = votes;
            _users = users;
            _user = user;
        }

        public async Task<Workspace> ImportAsync(IFormFile file)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                var fileBytes = stream.ToArray();
                stream.Seek(0, SeekOrigin.Begin);

                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataset = reader.AsDataSet();
                    var table = dataset.Tables[0];
                    var headers = GetHeaders(table).ToArray();

                    // delete if exists.
                    var importKey = file.FileName.Replace(" ", string.Empty);
                    var found = await _workspaces.FindImportedAsync(importKey);
                    if (found != null)
                    {
                        throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_AlreadyExist, "Workspace with the same import filename alredy exists. You can delete the workspace when the status is 'Archived'.");
                    }

                    var stakeholders = await ImportStakeholdersAsync(importKey, headers, table);
                    return await BuildWorkspaceAsync(importKey, stakeholders, headers, table);
                }
            }
        }

        private async Task<Workspace> BuildWorkspaceAsync(string importKey, 
            IDictionary<string, string> stakeholders, 
            SurveyColumn[] columns, 
            DataTable table)
        {
            var criteriaColumns = columns.OfType<SurveyColumnCriterion>();
            if (columns.Any() == false)
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidCriteria, "No criteria found.");
            }

            var criteriaAlternatives = criteriaColumns.GroupBy(x => x.Number);
            var firstCriterion = criteriaAlternatives.First();
            foreach (var kvp in criteriaAlternatives)
            {
                if (kvp.Count() != firstCriterion.Count())
                {
                    throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidCriteria, $"Alternatives count mismatch between criteria '{firstCriterion.Key}' and '{kvp.Key}'.");
                }
            }

            // criteria.
            var workspace = new Workspace() { ImportKey = importKey, Title = importKey, UserId = _user.UserId };
            workspace.Questionnaire.SetSettings(QuestionnaireSettings.Default());

            var numberToCriterion = new Dictionary<int, Criterion>();
            foreach (var g in criteriaAlternatives.OrderBy(x => x.Key))
            {
                var header = g.First();
                var criterion = workspace.AddCriterion(header.CriterionTitle);
                numberToCriterion[g.Key] = criterion;
            }

            // alternatives.
            var textToAlternative = new Dictionary<string, Alternative>();
            foreach (var item in criteriaAlternatives.First().ToArray())
            {
                var alternative = workspace.AddAlternative(item.AlternativeTitle);
                textToAlternative[item.AlternativeKey] = alternative;
            }

            workspace.ChangeStatus(WorkspaceStatus.Published);
            workspace.ChangeStatus(WorkspaceStatus.AcceptingVotes);
            await _workspaces.AddAsync(workspace);
            await _workspaces.UnitOfWork.SaveChangesAsync();

            // votes
            var criteriaWeightsNumberToIndex = columns.OfType<SurveyColumnCriterionWeight>().GroupBy(x => x.Number).ToDictionary(x => x.Key, x => x.First().ColumnIndex);
            var columnId = GetStakeholderIdColumn(columns);
            for (int rowIndex = 1; rowIndex < table.Rows.Count; rowIndex++)
            {
                var row = table.Rows[rowIndex];

                // find user.
                var stakeholderId = GetStakeholderId(table, columnId, row);
                var userId = stakeholders[stakeholderId];
                var vote = BuildVote(criteriaColumns, workspace, numberToCriterion, textToAlternative, row, stakeholderId, userId);

                // importance.
                foreach (var kvp in criteriaWeightsNumberToIndex)
                {
                    var criterion = numberToCriterion[kvp.Key];
                    var importance = row[kvp.Value]?.ToString();
                    if (int.TryParse(importance, out var importanceWeight) == false)
                    {
                        throw new ImportException(Domain.Common.DomainErrors.WorkspaceStatus_CannotFindCriterionWeight, $"Cannot find criterion weight for 'user:{stakeholderId}' and 'criterion:{kvp.Key}'.");
                    }

                    vote.CriteriaImportance.Add(new StakeholderCriterionImportance()
                    {
                        Vote = vote,
                        CriterionId = criterion.Id,
                        Weight = importanceWeight
                    });
                }

                await _votes.AddAsync(vote);
            }

            workspace.ChangeStatus(WorkspaceStatus.Finalized);
            await _votes.UnitOfWork.SaveChangesAsync();
            return workspace;
        }

        private static StakeholderVote BuildVote(IEnumerable<SurveyColumnCriterion> criteriaColumns, 
            Workspace workspace, Dictionary<int, Criterion> numberToCriterion, 
            Dictionary<string, Alternative> textToAlternative, DataRow row, string stakeholderId, string userId)
        {
            // vote.
            var vote = new StakeholderVote
            {
                WorkspaceId = workspace.Id,
                ApplicationUserId = userId
            };

            foreach (var column in criteriaColumns)
            {
                if (numberToCriterion.TryGetValue(column.Number, out var criterion) == false)
                {
                    throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidVoteValue, $"Cannot find criterion for 'user:{stakeholderId}' and 'criterion:{column.CriterionTitle}'.");
                }

                if (textToAlternative.TryGetValue(column.AlternativeKey, out var alternative) == false)
                {
                    throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidVoteValue, $"Cannot find alternative for 'user:{stakeholderId}' and 'alternative:{column.AlternativeTitle}'.");
                }

                var value = row[column.ColumnIndex]?.ToString();
                if (double.TryParse(value, out var voteValue) == false)
                {
                    throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidVoteValue, $"Cannot parse vote value for 'user:{stakeholderId}' and 'header:{column.RawTitle}'.");
                }

                var answer = new StakeholderAnswer() { Vote = vote, Alternative = alternative, Criterion = criterion, Value = voteValue };
                vote.Answers.Add(answer);
            }

            return vote;
        }

        private async Task<IDictionary<string, string>> ImportStakeholdersAsync(string workspaceKey, SurveyColumn[] columns, DataTable table)
        {
            var result = new Dictionary<string, string>();

            var columnId = GetStakeholderIdColumn(columns);
            var columnCountry = GetCountryColumn(columns);
            var columnWork = GetWorkColumn(columns);

            for (int rowIndex = 1; rowIndex < table.Rows.Count; rowIndex++)
            {
                var row = table.Rows[rowIndex];

                string id = GetStakeholderId(table, columnId, row);
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidStakeholderId, $"Could not parse stakeholder id for 'row:{rowIndex}'.");
                }

                var userId = $"{workspaceKey}_{id}";
                var user = IdentityFactory.BuildUser(new UserCredentials { Id = userId, Email = $"{userId}@email.com", Password = $"{id}!{id}!{DateTime.Now:yyyyMMdd}" });
                user.CountryId = GetStakeholderCountryId(columnCountry, row);
                user.JobCategoryId = GetStakeholderJobCategoryId(columnWork, row);
                await _users.AddAsync(user);
                await _users.AddUserToRoleAsync(userId, RoleNames.Stakeholder);
                result[id] = userId;
            }

            await _users.UnitOfWork.SaveChangesAsync();
            return result;
        }

        #region [ Work ]
        private int? GetStakeholderJobCategoryId(SurveyColumnCategory column, DataRow row)
        {
            var excelTitleToLower = CategoryToLower(column, row);
            if (string.IsNullOrEmpty(excelTitleToLower))
            {
                return null;
            }

            if (_workMap == null)
            {
                _workMap = new Dictionary<string, int>();
            }

            if (_workMap.TryGetValue(excelTitleToLower, out var id))
            {
                return id;
            }

            var distances = _allJobCategories.ToDictionary(x => x.Id, x => Fastenshtein.Levenshtein.Distance(excelTitleToLower, x.Title.ToLower()));
            var closest = distances.OrderBy(x => x.Value).FirstOrDefault();
            if (closest.Value < 3)
            {
                return closest.Key;
            }

            return null;
        }

        private SurveyColumnCategory GetWorkColumn(SurveyColumn[] columns)
        {
            return columns.OfType<SurveyColumnCategory>().FirstOrDefault(x => x.IsWork);
        }

        private static string CategoryToLower(SurveyColumnCategory column, DataRow row)
        {
            return row.ItemArray[column.ColumnIndex]?.ToString()?.Trim()?.ToLower();
        }
        #endregion

        #region [ Country ]
        private string GetStakeholderCountryId(SurveyColumnCategory columnCountry, DataRow row)
        {
            var countryToLower = CategoryToLower(columnCountry, row);
            if (string.IsNullOrEmpty(countryToLower))
            {
                return null;
            }

            if (_countriesMap == null)
            {
                _countriesMap = new Dictionary<string, string>();
            }

            if (_countriesMap.TryGetValue(countryToLower, out var id))
            {
                return id;
            }

            var distances = _allCountries.ToDictionary(x => x.Id, x => Fastenshtein.Levenshtein.Distance(countryToLower, x.Title));
            var closest = distances.OrderBy(x => x.Value).FirstOrDefault();
            if (closest.Value < 3)
            {
                return closest.Key;
            }

            return null;
        }

        private SurveyColumnCategory GetCountryColumn(SurveyColumn[] columns)
        {
            return columns.OfType<SurveyColumnCategory>().FirstOrDefault(x => x.IsCountry);
        }
        #endregion

        private static string GetStakeholderId(DataTable table, SurveyColumnStakeholderId columnId, DataRow row)
        {
            return row[columnId.ColumnIndex]?.ToString()?.Trim();
        }

        private static SurveyColumnStakeholderId GetStakeholderIdColumn(SurveyColumn[] headers)
        {
            var columns = headers.OfType<SurveyColumnStakeholderId>().FirstOrDefault();
            if (columns == null)
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidStakeholderId, "Column for stakeholders ids not found.");
            }

            return columns;
        }

        private IEnumerable<SurveyColumn> GetHeaders(DataTable table)
        {
            var row = table.Rows[0];
            for (int i = 0; i < row.ItemArray.Length; i++)
            {
                yield return SurveyColumn.From(i, row.ItemArray[i]?.ToString());
            }
        }
    }
}
