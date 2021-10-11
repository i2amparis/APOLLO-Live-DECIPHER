using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Topsis.Web.Infrastructure
{
    public class CheckRoleHandler : AuthorizationHandler<CheckRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       CheckRoleRequirement requirement)
        {
            var isAuthorized = context.User.IsInRole(requirement.RoleName);
            if (isAuthorized)
            {
                context.Succeed(requirement);
            }
            
            return Task.CompletedTask;
        }
    }

    public class CheckRoleRequirement : IAuthorizationRequirement
    {
        public CheckRoleRequirement(string roleName)
        {
            RoleName = roleName;
        }

        public string RoleName { get; }
    }
}
