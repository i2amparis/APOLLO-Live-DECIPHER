using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public async Task<Workspace> FindImportedAsync(string importKey)
        {
            return await Set.FirstOrDefaultAsync(x => x.ImportKey == importKey);
        }

        public async Task ClearVotesAndReportsAsync(int id)
        {
            var votes = await _db.WsStakeholderVotes
                .Include(x => x.CriteriaImportance)
                .Include(x => x.Answers)
                .Where(x => x.WorkspaceId == id).ToArrayAsync();

            _db.WsStakeholderVotes.RemoveRange(votes);

            var reports = await _db.WsWorkspacesReports
                .Where(x => x.WorkspaceId == id)
                .ToArrayAsync();

            _db.WsWorkspacesReports.RemoveRange(reports);
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

        public async override Task DeleteAsync(Workspace entity)
        {
            var workspace = await Set
                .Include(x => x.Questionnaire)
                    .ThenInclude(x => x.Criteria)
                .Include(x => x.Questionnaire)
                    .ThenInclude(x => x.Alternatives)
                .Include(x => x.Votes)
                    .ThenInclude(x => x.Answers)
                .Include(x => x.Reports)
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            foreach (var item in workspace.Votes)
            {
                _db.Users.Remove(new Application.Contracts.Identity.ApplicationUser { Id = item.ApplicationUserId });
            }

            Set.Remove(workspace);
        }
    }
}
