using System;
using Topsis.Domain.Contracts;

namespace Topsis.Domain
{
    public class Moderator : Entity<string>, IHaveIdentity, IHaveDemographics, ICanTrackCreation
    {
        public Moderator()
        {
            Id = EntityFactory.NewId();
        }

        public string Email { get; set; }
        public string Password { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }

        public ChangeTrack Created { get; set; }
    }
}
