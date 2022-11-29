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

        private const string ModeratorUserId = "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA71";
        private const string ModeratorUserPassword = "!!4921Rfbw3!";
        private const string ModeratorEmail = "kkoasidis@epu.ntua.gr";

        public static void ApplyTo(ModelBuilder builder)
        {
            //builder.Entity<ApplicationRole>().HasData(Roles());
            //builder.Entity<ApplicationUser>().HasData(Users());
            //builder.Entity<IdentityUserRole<string>>().HasData(UsersInRoles());
        }

        public static void ApplyTo(WorkspaceDbContext db)
        {
            foreach (var item in GetRoles())
            {
                db.Roles.Add(item);
            }

            foreach (var item in GetUsers())
            {
                db.Users.Add(item);
            }

            foreach (var item in GetUsersInRoles())
            {
                db.UserRoles.Add(item);
            }

            db.SaveChanges();
        }

        private static IEnumerable<ApplicationRole> GetRoles()
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

        private static IEnumerable<ApplicationUser> GetUsers()
        {
            yield return new UserCredentials() 
            { 
                Id = AdminUserId, 
                Email = AdminEmail, 
                Password = AdminUserPassword 
            }.BuildUser();

            yield return new UserCredentials()
            {
                Id = ModeratorUserId,
                Email = ModeratorEmail,
                Password = ModeratorUserPassword
            }.BuildUser();
        }

        private static IEnumerable<IdentityUserRole<string>> GetUsersInRoles()
        {
            // root user as admin,moderator,stakeholder.
            yield return BuildUserRole(RoleNames.Admin, AdminUserId);
            yield return BuildUserRole(RoleNames.Moderator, AdminUserId);
            yield return BuildUserRole(RoleNames.Stakeholder, AdminUserId);

            yield return BuildUserRole(RoleNames.Moderator, ModeratorUserId);
            yield return BuildUserRole(RoleNames.Stakeholder, ModeratorUserId);
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