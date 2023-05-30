using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Repositories
{
    public class WorkspaceReportRepository : BaseRepository<int, WorkspaceReport>, 
        IWorkspaceReportRepository
    {
        public WorkspaceReportRepository(WorkspaceDbContext db) : base(db)
        {
        }

        public async Task<WorkspaceReport> FindAsync(int workspaceId, FeedbackRound round)
        {
            return await _db.WsWorkspacesReports
                .SingleOrDefaultAsync(x => x.WorkspaceId == workspaceId
                                        && x.Round == round);
        }
    }
}
