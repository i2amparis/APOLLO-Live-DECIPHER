using Topsis.Domain.Contracts;

namespace Topsis.Domain
{
    public class UserCredentials : Entity<string>, IHaveIdentity
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
