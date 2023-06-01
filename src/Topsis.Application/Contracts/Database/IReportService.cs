using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Contracts.Database
{
    public interface IReportService
    {
        ValueTask<WorkspaceReport> GetResultsAsync(string workspaceId);

        ValueTask<StakeholderAnswerDto[]> GetAnswersForCalculationAsync(int workspaceId);

        ValueTask<IList<StakeholderWorkspaceDto>> GetStakeholderWorkspacesAsync(IUserContext user, bool includeOld = false);

        ValueTask<Country> GetCountryAsync(string id);

        ValueTask<StakeholderVoteViewModel> GetStakeholderVoteViewModelAsync(IUserContext user, string workspaceId);
        ValueTask<WorkspaceReportViewModel> GetWorkspaceReportAsync(string id, IUserContext userContext);
        ValueTask<IDictionary<int,string>> GetJobCategoriesAsync();

        public void ClearWorkspaceCache(int workspaceId);
        StakeholderDemographicsDto[] GetStakeholdersDemographicsAsync(string[] userIds);
        Task<PaginatedList<ApplicationUser>> GetUsersAsync(string term = null, int page = 1, int pageSize = 20);
        void AddUserVoteToCache(StakeholderVote vote);
    }
}
