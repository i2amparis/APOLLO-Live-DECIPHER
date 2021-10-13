using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Caching;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;
using Topsis.Domain.Common;
using Topsis.Domain.Contracts;

namespace Topsis.Adapters.Database.Reports
{
    public class ReportService : IReportService
    {
        private const int StakeholderVoteViewModelCacheIntervalInSeconds = 30;
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

            //var stakeholderVm = await _cache.GetOrCreateAsync($"__stakeholder_vote_{id}",
            //    () => BuildStakeholderViewModelAsync(id),
            //    slidingExpiration: TimeSpan.FromSeconds(StakeholderVoteViewModelCacheIntervalInSeconds));

            var stakeholderVm = await BuildStakeholderViewModelAsync(id);

            var allAnswers = await (from a in _db.WsStakeholderAnswers
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

            //var answers = await _db.WsStakeholderAnswers
            //    .Include(x => x.Vote)
            //    .Where(x => x.Vote.WorkspaceId == id && x.Vote.ApplicationUserId == user.UserId)
            //    .ToArrayAsync();
            var answers = allAnswers.Any() 
                ? allAnswers.GroupBy(x => x.VoteId).OrderBy(x => x.Key).LastOrDefault().ToList()
                : new List<StakeholderAnswerDto>();
            stakeholderVm.AddStakeholderAnswers(answers);
            return stakeholderVm;
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
                AlternativesOrdered = workspace.Questionnaire.Alternatives.OrderBy(x => x.Order).Select(x => new KeyValuePair<int, string>(x.Id, x.Title)).ToArray()
            };
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

                // todo cache.
                var workspace = await _db.WsWorkspaces
                    .Include(x => x.Reports)
                    .Include(x => x.Questionnaire)
                        .ThenInclude(x => x.Alternatives)
                    .SingleOrDefaultAsync(x => x.Id == workspaceId);

                return new WorkspaceReportViewModel(workspace, user);
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
    }
}
