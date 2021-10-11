using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain;
using Topsis.Domain.Contracts;

namespace Topsis.Adapters.Database.Seed
{
    public static class IdentitySeed
    {
        // https://stackoverflow.com/questions/50785009/how-to-seed-an-admin-user-in-ef-core-2-1-0
        private const string AdminUserId = "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA70";
        private const string AdminUserPassword = "!!4921Rfaw3!";
        private const string AdminEmail = "a.soursos@gmail.com";

        public static void ApplyTo(ModelBuilder builder)
        {
            builder.Entity<ApplicationRole>().HasData(Roles());
            builder.Entity<ApplicationUser>().HasData(Users());
            builder.Entity<IdentityUserRole<string>>().HasData(UsersInRoles());
        }

        private static IEnumerable<ApplicationRole> Roles()
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

        private static IEnumerable<ApplicationUser> Users()
        {
            yield return new Administrator() 
            { 
                Id = AdminUserId, 
                Email = AdminEmail, 
                Password = AdminUserPassword 
            }.BuildUser();
        }

        private static IEnumerable<IdentityUserRole<string>> UsersInRoles()
        {
            // root user as admin,moderator,stakeholder.
            yield return BuildUserRole(RoleNames.Admin, AdminUserId);
            yield return BuildUserRole(RoleNames.Moderator, AdminUserId);
            yield return BuildUserRole(RoleNames.Stakeholder, AdminUserId);
        }

        private static IdentityUserRole<string> BuildUserRole(string role, string userId)
        {
            return new IdentityUserRole<string>
            {
                RoleId = role.GetRoleId(),
                UserId = userId
            };
        }
    }
}