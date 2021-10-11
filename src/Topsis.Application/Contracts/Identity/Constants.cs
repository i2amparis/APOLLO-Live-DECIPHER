using System;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Contracts.Identity
{
    public static class Constants
    {
        private const string AdminRoleId = "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7A";
        private const string ModeratorRoleId = "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7B";
        private const string StakeholderRoleId = "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7C";

        public static string GetRoleId(this string roleName)
        {
            return roleName switch
            {
                RoleNames.Admin => AdminRoleId,
                RoleNames.Moderator => ModeratorRoleId,
                RoleNames.Stakeholder => StakeholderRoleId,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
