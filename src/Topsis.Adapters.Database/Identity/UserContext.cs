using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using Topsis.Domain.Contracts;

namespace Topsis.Adapters.Database.Identity
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId => _httpContextAccessor.HttpContext.GetClaimValue(ClaimTypes.NameIdentifier); 
    }

    public static class HttpContextExtensions
    {
        public static string GetClaimValue(this HttpContext httpContext, string type)
        {
            return httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
