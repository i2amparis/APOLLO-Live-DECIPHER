using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Repositories
{
    public class VoteRepository : BaseRepository<int, StakeholderVote>, IVoteRepository
    {
        public VoteRepository(WorkspaceDbContext db) : base(db)
        {
        }

        public override Task<StakeholderVote> GetByIdAsync(int id)
        {
            return _db.WsStakeholderVotes
                .Include(x => x.Answers)
                .SingleAsync(x => x.Id == id);
        }
    }
}
