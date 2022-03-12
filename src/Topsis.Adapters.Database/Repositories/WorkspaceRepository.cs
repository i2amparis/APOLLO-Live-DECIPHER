using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
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

        public async Task<string[]> ClearVotesAndReportsAsync(int id)
        {
            var votes = await _db.WsStakeholderVotes
                .Include(x => x.CriteriaImportance)
                .Include(x => x.Answers)
                .Where(x => x.WorkspaceId == id).ToArrayAsync();

            var userIds = votes.Select(x => x.ApplicationUserId).Distinct().ToArray();

            _db.WsStakeholderVotes.RemoveRange(votes);

            var reports = await _db.WsWorkspacesReports
                .Where(x => x.WorkspaceId == id)
                .ToArrayAsync();

            _db.WsWorkspacesReports.RemoveRange(reports);

            return userIds;
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
            var userids = await ClearVotesAndReportsAsync(entity.Id);
            //await _db.SaveChangesAsync();

            var roles = await _db.UserRoles.Where(x => userids.Contains(x.UserId)).ToArrayAsync();
            _db.RemoveRange(roles);
            //await _db.SaveChangesAsync();

            var users = await _db.Users.Where(x => userids.Contains(x.Id)).ToArrayAsync();
            _db.Users.RemoveRange(users);
            //await _db.SaveChangesAsync();

            var workspace = await Set
                .Include(x => x.Questionnaire)
                    .ThenInclude(x => x.Criteria)
                .Include(x => x.Questionnaire)
                    .ThenInclude(x => x.Alternatives)
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            Set.Remove(workspace);
        }
    }
}
