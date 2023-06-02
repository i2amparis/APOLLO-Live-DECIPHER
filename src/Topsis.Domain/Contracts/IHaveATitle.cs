using System;

namespace Topsis.Domain.Contracts
{
    public interface IHaveATitle
    {
        string Title { get; }
    }

    public interface ICanBeOrdered : IEntity<int>
    {
        short Order { get; set; }
    }

    public interface IUserContext
    {
        string UserId { get; }
        bool IsInRole(string role);
    }

    public interface IHaveIdentity
    { 
        string Id { get; }
        string Email { get; }
        string Password { get; }
    }

    public interface IStakeholderProfile
    { 
        Country Country { get; set; }
        JobCategory JobCategory { get; set; }
    }

    public interface IHaveDemographics
    {
        string LastName { get; set; }
        string FirstName { get; set; }
    }

    public interface ICanTrackCreation
    { 
        ChangeTrack Created { get; set; }
    }

    public class ChangeTrack
    {
        private ChangeTrack() { /* ef core */ }

        public ChangeTrack(string userId, DateTime? time = null)
        {
            UserId = userId;
            Time = time ?? DateTime.UtcNow;
        }

        public string UserId { get; private set; }
        public DateTime Time { get; private set; }
    }
}
