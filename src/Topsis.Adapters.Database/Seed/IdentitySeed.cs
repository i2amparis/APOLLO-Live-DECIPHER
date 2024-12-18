using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain;
using Topsis.Domain.Contracts;

namespace Topsis.Adapters.Database.Seed
{
    public static class IdentitySeed
    {
        // https://stackoverflow.com/questions/50785009/how-to-seed-an-admin-user-in-ef-core-2-1-0

        public static async Task<bool> IsInitializedAsync(this WorkspaceDbContext db)
        {
            return await db.UserRoles.AnyAsync();
        }

        public static async Task InitializeAsync(this WorkspaceDbContext db, UserCredentials adminCredentials)
        {
            var roles = BuildRoles();
            db.Roles.AddRange(roles);

            var adminUser = adminCredentials.BuildUser();
            await db.Users.AddRangeAsync(adminUser);

            var adminRoles = BuildUserRoles(adminUser.Id, RoleNames.Admin, RoleNames.Moderator, RoleNames.Stakeholder);
            await db.UserRoles.AddRangeAsync(adminRoles);

            await db.SaveChangesAsync();
        }

        private static IEnumerable<ApplicationRole> BuildRoles()
        {
            yield return BuildRole(RoleNames.Admin);
            yield return BuildRole(RoleNames.Moderator);
            yield return BuildRole(RoleNames.Stakeholder);
        }

        private static ApplicationRole BuildRole(string roleName)
        {
            return new ApplicationRole
            {
                Id = roleName.GetRoleId(),
                Name = roleName,
                NormalizedName = roleName.ToUpper()
            };
        }

        private static IEnumerable<IdentityUserRole<string>> BuildUserRoles(string userId, params string[] roles)
        {
            foreach (var role in roles)
            {
                yield return new IdentityUserRole<string>
                {
                    RoleId = role.GetRoleId(),
                    UserId = userId
                };
            }
        }
    }
}