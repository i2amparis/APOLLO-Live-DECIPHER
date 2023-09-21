using AutoMapper;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Features
{
    public static class CreateModerator
    {
        public class Command : IRequest<string>, IHaveDemographics
        {
            public Command(string email, string password, string firstName, string lastName)
            {
                Email = email;
                Password = password;
                FirstName = firstName;
                LastName = lastName;
            }

            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(m => m.Email).NotNull().Length(0, 50);
                RuleFor(s => s.Email)
                    .NotEmpty().WithMessage("Email address is required")
                    .EmailAddress().WithMessage("A valid email is required")
                    .Length(5, 50);
                RuleFor(m => m.LastName).NotNull().Length(2, 50);
                RuleFor(m => m.FirstName).NotNull().Length(2, 50);
            }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly IApplicationUserRepository<ApplicationUser> _users;
            private readonly IUserContext _userContext;
            private readonly IMapper _map;

            public Handler(IApplicationUserRepository<ApplicationUser> users, 
                IUserContext userContext,
                IMapper map)
            {
                _users = users;
                _userContext = userContext;
                _map = map;
            }

            public async Task<string> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var moderator = _map.Map<Moderator>(cmd);
                var user = moderator.BuildUser(_userContext);
                user = await _users.AddAsync(user);
                await _users.AddUserToRoleAsync(user.Id, RoleNames.Moderator);

                await _users.UnitOfWork.SaveChangesAsync();

                return user.Id;
            }
        }
    }
}
