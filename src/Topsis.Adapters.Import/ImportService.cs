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
                    var workspaceKey = Guid.NewGuid().ToString();
                    var stakeholders = ImportStakeholders(workspaceKey, headers, table);
                    return await BuildWorkspaceAsync(workspaceKey, stakeholders, headers, table);
                }
            }
        }

        private async Task<Workspace> BuildWorkspaceAsync(string workspaceKey, 
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
            var workspace = new Workspace() { Title = workspaceKey, UserId = _user.UserId };
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

            await _workspaces.AddAsync(workspace);
            await _workspaces.UnitOfWork.SaveChangesAsync();

            // votes
            var criteriaWeightsNumberToIndex = columns.OfType<SurveyColumnCriterionWeight>().GroupBy(x => x.Number).ToDictionary(x => x.Key, x => x.First().ColumnIndex);
            var columnId = GetStakeholderIdColumn(columns);
            for (int rowIndex = 1; rowIndex < table.Rows.Count; rowIndex++)
            {
                // find user.
                var stakeholderId = GetStakeholderId(table, columnId, rowIndex);
                var userId = stakeholders[stakeholderId];

                var row = table.Rows[rowIndex];
                StakeholderVote vote = BuildVote(criteriaColumns, workspace, numberToCriterion, textToAlternative, row, stakeholderId, userId);

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

        private IDictionary<string, string> ImportStakeholders(string workspaceKey, SurveyColumn[] columns, DataTable table)
        {
            var result = new Dictionary<string, string>();

            var columnId = GetStakeholderIdColumn(columns);

            for (int rowIndex = 1; rowIndex < table.Rows.Count; rowIndex++)
            {
                string id = GetStakeholderId(table, columnId, rowIndex);
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidStakeholderId, $"Could not parse stakeholder id for 'row:{rowIndex}'.");
                }

                var userId = $"{workspaceKey}_{id}";
                var user = IdentityFactory.BuildUser(new UserCredentials { Id = userId, Email = $"{userId}@email.com", Password = $"{id}!{id}!{DateTime.Now:d}" });
                _users.AddAsync(user);
                _users.AddUserToRoleAsync(userId, RoleNames.Stakeholder);
                result[id] = userId;
            }

            _users.UnitOfWork.SaveChangesAsync();
            return result;
        }

        private static string GetStakeholderId(DataTable table, SurveyColumnStakeholderId columnId, int rowIndex)
        {
            return table.Rows[rowIndex][columnId.ColumnIndex]?.ToString()?.Trim();
        }

        private static SurveyColumnStakeholderId GetStakeholderIdColumn(SurveyColumn[] headers)
        {
            var header = headers.OfType<SurveyColumnStakeholderId>().FirstOrDefault();
            if (header == null)
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidStakeholderId, "Column for stakeholders ids not found.");
            }

            return header;
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
