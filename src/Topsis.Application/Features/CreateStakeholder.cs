using AutoMapper;
using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Features
{
    public class CreateStakeholder
    {
        public class Command : IRequest<ApplicationUser>, IStakeholderProfile
        {
            //[Required(ErrorMessage = "Please select your country.")]
            public Country Country { get; set; }

            public JobCategory JobCategory { get; set; }
            public Gender Gender { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(m => m.Country).NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, ApplicationUser>
        {
            private readonly IApplicationUserRepository<ApplicationUser> _users;

            public Handler(IApplicationUserRepository<ApplicationUser> users)
            {
                _users = users;
            }

            public async Task<ApplicationUser> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var user = cmd.BuildUser();
                user = await _users.AddAsync(user);
                await _users.AddUserToRoleAsync(user.Id, RoleNames.Stakeholder);

                await _users.UnitOfWork.SaveChangesAsync();

                return user;
            }
        }
    }
}
