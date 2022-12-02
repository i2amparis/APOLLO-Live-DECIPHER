using System.Collections.Generic;

namespace Topsis.Domain.Contracts
{
    public static class RoleNames
    {
        public const string Admin = "admin";
        public const string Moderator = "moderator";
        public const string Stakeholder = "stakeholder";

        public static IDictionary<string, string> All()
        {
            return new Dictionary<string, string>
            {
                { Admin, "Can manage users." },
                { Moderator, "Can moderate workspaces." },
                { Stakeholder, "Can answer/vote a questionnaire." }
            };
        }
    }
}
