using Microsoft.AspNetCore.Identity;
using System;
using Topsis.Application.Features;
using Topsis.Domain;
using Topsis.Domain.Common;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Contracts.Identity
{
    public static class IdentityFactory
    {

        public static ApplicationUser BuildUser(this CreateStakeholder.Command cmd, IUserContext creator = null)
        {
            var id = EntityFactory.NewId();
            var result = BuildUser(id, email: $"{id}@ntua.gr", password: id.GetHashCode().Hash(), creator);

            result.CountryId = cmd.Country.Id;
            result.JobCategoryId = cmd.JobCategory.Id;

            return result;
        }

        public static ApplicationUser BuildUser(this IHaveIdentity identity, IUserContext creator = null)
        {
            return BuildUser(identity.Id, identity.Email, identity.Password, creator);
        }

        private static ApplicationUser BuildUser(string id, string email, string password, IUserContext creator = null)
        {
            var result = new ApplicationUser
            {
                Id = id,
                UserName = email,
                NormalizedUserName = email.Normalize(),
                Email = email,
                NormalizedEmail = email.Normalize(),
                PhoneNumber = "",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            result.PasswordHash = GeneratePassword(result, password);

            if (creator != null)
            {
                result.Created = new ChangeTrack(creator.UserId);
            }

            return result;
        }

        private static string GeneratePassword(ApplicationUser user, string password)
        {
            var passHash = new PasswordHasher<ApplicationUser>();
            return passHash.HashPassword(user, password);
        }
    }
}
