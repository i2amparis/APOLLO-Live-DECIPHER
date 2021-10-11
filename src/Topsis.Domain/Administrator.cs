using Topsis.Domain.Contracts;

namespace Topsis.Domain
{
    public class Administrator : Entity<string>, IHaveIdentity
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
