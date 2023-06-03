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

            if (cmd.Country != null)
            {
                result.CountryId = cmd.Country.Id;
            }

            if (cmd.JobCategory != null)
            {
                result.JobCategoryId = cmd.JobCategory.Id;
            }

            if (cmd.Gender != Gender.Unknown)
            {
                result.GenderId = cmd.Gender;
            }

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
                NormalizedUserName = email.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                PhoneNumber = "",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
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
