using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository<ApplicationUser>
    {
        private readonly WorkspaceDbContext _db;

        public ApplicationUserRepository(WorkspaceDbContext db)
        {
            _db = db;
        }

        public IUnitOfWork UnitOfWork => _db;

        public async Task<ApplicationUser> AddAsync(ApplicationUser user)
        {
            var result = await _db.Users.AddAsync(user);
            return result.Entity;
        }

        public async Task AddUserToRoleAsync(string userId, string roleName)
        {
            var roles = _db.Roles.ToArray();
            var role = await _db.Roles.Where(x => x.NormalizedName == EntityFactory.Normalize(roleName))
                .FirstOrDefaultAsync();
            if (role == null)
            {
                throw new KeyNotFoundException();
            }

            if (_db.UserRoles.Any(x => x.UserId == userId && x.RoleId == role.Id))
            {
                return;
            }

            var user = await _db.Users.FindAsync(userId);
            await _db.UserRoles.AddAsync(new IdentityUserRole<string>() { RoleId = role.Id, UserId = userId });
        }

        public Task DeleteAsync(ApplicationUser entity)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task<IPaginatedList<ApplicationUser>> GetPagedReponseAsync(int page, int size)
        {
            return await PaginatedList<ApplicationUser>.CreateAsync(_db.Users, page, size);
        }

        public Task<IReadOnlyList<ApplicationUser>> ListAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(ApplicationUser entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
