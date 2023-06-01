using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Caching;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain;
using Topsis.Domain.Common;
using Topsis.Domain.Contracts;

namespace Topsis.Adapters.Database.Reports
{
    public class ReportService : IReportService
    {
        private const int StakeholderVoteViewModelCacheIntervalInSeconds = 300;
        private const int WorkspaceReportCacheIntervalInSeconds = 300;
        private const int StakeholderUserVoteIdCacheIntervalInSeconds = 300;
        private readonly WorkspaceDbContext _db;
        private readonly ICachingService _cache;

        public ReportService(WorkspaceDbContext db, ICachingService cache)
        {
            _db = db;
            _cache = cache;
        }

        public async ValueTask<StakeholderAnswerDto[]> GetAnswersForCalculationAsync(int workspaceId)
        {
            return await _db.ReportStakeholderAnswers
                .Where(x => x.WorkspaceId == workspaceId)
                .ToArrayAsync();
        }

        public async ValueTask<Country> GetCountryAsync(string id)
        {
            return await _db.WsCountries.FindAsync(id);
        }

        public async ValueTask<WorkspaceReport> GetResultsAsync(string workspaceId)
        {
            var id = workspaceId.DehashInts().First();
            return await _db.WsWorkspacesReports
                .Where(x => x.Id == id)
                .LastOrDefaultAsync();
        }

        public async ValueTask<StakeholderVoteViewModel> GetStakeholderVoteViewModelAsync(IUserContext user, string workspaceId)
        {
            var id = workspaceId.DehashInts().First();

            var stakeholderVm = await _cache.GetOrCreateAsync(GetStakeholderVoteViewModelCacheKey(id),
                () => BuildStakeholderViewModelAsync(id),
                slidingExpiration: TimeSpan.FromSeconds(StakeholderVoteViewModelCacheIntervalInSeconds));

            var answers = await GetStakeholderAnswersAsync(user, id);

            var report = await GetWorkspaceReportAsync(workspaceId, user);
            stakeholderVm.AddStakeholderAnswers(answers, report);
            return stakeholderVm;
        }

        public async ValueTask<IList<StakeholderWorkspaceDto>> GetStakeholderWorkspacesAsync(IUserContext user, bool includeOld)
        {
            if (string.IsNullOrEmpty(user?.UserId))
            {
                return await _db.ReportStakeholderWorkspaces
                .Where(x => x.StakeholderId == null)
                .ToArrayAsync();
            }

            var results = await _db.ReportStakeholderWorkspaces
                .Where(x => x.StakeholderId == null || x.StakeholderId == user.UserId)
                .ToListAsync();

            // remove duplicate items that stakeholder has voted.
            var removed = results.GroupBy(x => x.WorkspaceId).Where(x => x.Count() > 1)
                .SelectMany(x => x.Where(x => x.StakeholderId == null))
                .ToArray();

            foreach (var item in removed)
            {
                results.Remove(item);
            }

            return results;
        }

        public async ValueTask<WorkspaceReportViewModel> GetWorkspaceReportAsync(string id, IUserContext user)
        {
            try
            {
                int workspaceId = id.DehashInts().First();
                var workspace = await _cache.GetOrCreateAsync(GetWorkspaceReportCacheKey(workspaceId),
                    () => LoadWorkspaceForReportAsync(workspaceId),
                    slidingExpiration: TimeSpan.FromSeconds(WorkspaceReportCacheIntervalInSeconds));
                
                var userVoteId = await GetUserVoteAsync(user?.UserId, workspaceId);
                return new WorkspaceReportViewModel(workspace, user, userVoteId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async ValueTask<IDictionary<int, string>> GetJobCategoriesAsync()
        {
            return await _db.WsJobCategories.ToDictionaryAsync(x => x.Id, x => x.Title);
        }

        public void ClearWorkspaceCache(int workspaceId)
        {
            var key1 = GetStakeholderVoteViewModelCacheKey(workspaceId);
            _cache.Remove(key1);

            var key2 = GetWorkspaceReportCacheKey(workspaceId);
            _cache.Remove(key2);
        }

        #region [ User / Vote ]
        private static string GetUserVoteKey(string userId, int workspaceId)
        {
            return $"__user_vote_{userId}_{workspaceId}".ToLower();
        }

        public void AddUserVoteToCache(StakeholderVote vote)
        {
            _cache.Set(GetUserVoteKey(vote.ApplicationUserId, vote.WorkspaceId), 
                vote.Id, 
                TimeSpan.FromSeconds(StakeholderUserVoteIdCacheIntervalInSeconds));
        }

        public async Task<int?> GetUserVoteAsync(string userId, int workspaceId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var key = GetUserVoteKey(userId, workspaceId);
            var result = await _cache.GetOrCreateAsync(key, async () => { 
                var vote = await _db.WsStakeholderVotes.Where(x => x.ApplicationUserId == userId && x.WorkspaceId == workspaceId)
                    .FirstOrDefaultAsync();
                return vote?.Id;
            });
            return result;
        }
        #endregion

        #region [ Helpers ]
        #region [ Vote ]
        private static string GetStakeholderVoteViewModelCacheKey(int workspaceId)
        {
            return $"__stakeholder_vote_{workspaceId}";
        }

        private async Task<StakeholderVoteViewModel> BuildStakeholderViewModelAsync(int workspaceId)
        {

            var workspace = await _db.WsWorkspaces
                .Include(x => x.Questionnaire)
                    .ThenInclude(x => x.Criteria)
                .Include(x => x.Questionnaire)
                    .ThenInclude(x => x.Alternatives)
                .FirstOrDefaultAsync(x => x.Id == workspaceId);

            var settings = workspace.Questionnaire.GetSettings();

            return new StakeholderVoteViewModel
            {
                WorkspaceId = workspace.Id.Hash(),
                WorkspaceStatus = workspace.CurrentStatus,
                WorkspaceTitle = workspace.Title,
                AlternativeRange = settings.AlternativeRange,
                CriteriaImportanceRange = settings.CriteriaWeightRange,
                CriteriaOrdered = workspace.Questionnaire.Criteria.OrderBy(x => x.Order).ToArray(),
                AlternativesOrdered = workspace.Questionnaire.Alternatives.OrderBy(x => x.Order).Select(x => new KeyValuePair<int, string>(x.Id, x.Title)).ToArray(),
                Settings = workspace.Questionnaire.GetSettings()
            };
        }

        private async Task<List<StakeholderAnswerDto>> GetStakeholderAnswersAsync(IUserContext user, int id)
        {
            var allVotesAnswers = await (from a in _db.WsStakeholderAnswers
                                         join v in _db.WsStakeholderVotes on a.VoteId equals v.Id
                                         join i in _db.WsStakeholderCriteriaImportance on
                                            new { a.VoteId, a.CriterionId } equals new { i.VoteId, i.CriterionId }
                                         where v.WorkspaceId == id && v.ApplicationUserId == user.UserId
                                         select new StakeholderAnswerDto
                                         {
                                             VoteId = v.Id,
                                             AlternativeId = a.AlternativeId,
                                             CriterionId = a.CriterionId,
                                             CriterionWeight = i.Weight,
                                             AnswerValue = a.Value,
                                             StakeholderId = user.UserId,
                                             WorkspaceId = id
                                         }).ToListAsync();

            return allVotesAnswers.Any()
                ? allVotesAnswers.GroupBy(x => x.VoteId).OrderBy(x => x.Key).LastOrDefault().ToList()
                : new List<StakeholderAnswerDto>();
        }

        #endregion

        #region [ Report ]
        private string GetWorkspaceReportCacheKey(int workspaceId)
        {
            return $"__workspace_report_{workspaceId}";
        }

        private async Task<Workspace> LoadWorkspaceForReportAsync(int workspaceId)
        {
            return await _db.WsWorkspaces
                .Include(x => x.Reports)
                .Include(x => x.Questionnaire)
                    .ThenInclude(x => x.Alternatives)
                .SingleOrDefaultAsync(x => x.Id == workspaceId);
        }

        public StakeholderDemographicsDto[] GetStakeholdersDemographicsAsync(string[] userIds)
        {
            var query = (from id in userIds
                        join item in _db.Users on id equals item.Id
                        select new StakeholderDemographicsDto(item.Id, item.GenderId, item.JobCategoryId)).AsQueryable();

            return query.AsNoTracking().ToArray();
        }

        public async Task<PaginatedList<ApplicationUser>> GetUsersAsync(string term = null, int page = 1, int pageSize = 20)
        {
            var zeroBasedPage = 0;
            if (page > 0)
            {
                zeroBasedPage = page - 1;
            }

            if (pageSize < 1 || pageSize > 100)
            {
                pageSize = 20;
            }

            var query = _db.Users
                .OrderBy(x => x.Email)
                .AsQueryable();

            if (string.IsNullOrWhiteSpace(term) == false)
            {
                query = query.Where(x => x.Email.Contains(term) 
                                || (x.LastName != null && x.LastName.Contains(term))
                                || (x.FirstName != null && x.FirstName.Contains(term)));
            }

            return await PaginatedList<ApplicationUser>.CreateAsync(query, page, pageSize);
        }
        #endregion

        #endregion

    }
}
