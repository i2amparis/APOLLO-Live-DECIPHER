using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Repositories
{
    public class WorkspaceRepository : BaseRepository<int, Workspace>, IWorkspaceRepository
    {
        public WorkspaceRepository(WorkspaceDbContext db) : base(db)
        {
        }

        public override async Task<Workspace> GetByIdAsync(int id)
        {
            return await _db.WsWorkspaces
                .Include(x => x.Reports)
                .Include(x => x.Questionnaire)
                    .ThenInclude(x => x.Criteria)
                .Include(x => x.Questionnaire)
                    .ThenInclude(x => x.Alternatives)
                .SingleAsync(x => x.Id == id);
        }

        public async Task<Workspace> GetByIdForCalculationAsync(int id)
        {
            return await _db.WsWorkspaces
                .Include(x => x.Votes)
                .Include(x => x.Reports)
                .Include(x => x.Questionnaire)
                    .ThenInclude(x => x.Criteria)
                .Include(x => x.Questionnaire)
                    .ThenInclude(x => x.Alternatives)
                .SingleAsync(x => x.Id == id);
        }
    }
}
