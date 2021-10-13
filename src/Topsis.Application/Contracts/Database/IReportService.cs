using System.Collections.Generic;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Algorithm;
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
    }
}
