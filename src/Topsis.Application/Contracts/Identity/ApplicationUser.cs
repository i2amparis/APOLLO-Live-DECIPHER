using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Topsis.Domain;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Contracts.Identity
{
    public class ApplicationUser : IdentityUser<string>, IStakeholderProfile, ICanTrackCreation
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        public string CountryId { get; set; }
        public Country Country {get;set;}
        public int? JobCategoryId { get; set; }
        public JobCategory JobCategory { get; set; }
        public ChangeTrack Created { get; set; }

        public ICollection<StakeholderVote> Votes { get; set; }
    }
}
