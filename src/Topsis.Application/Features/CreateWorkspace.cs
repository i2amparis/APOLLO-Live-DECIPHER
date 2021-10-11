using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;
using Topsis.Domain.Common;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Features
{
    public static class CreateWorkspace
    {
        public class Command : IRequest<string>
        {
            [Required]
            [StringLength(255, MinimumLength =3)]
            public string Title { get; set; }
            [Range(1,50)]
            public int CriteriaNo { get; set; }
            [Range(1, 50)]
            public int AlternativesNo { get; set; }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly IWorkspaceRepository _workspaces;
            private readonly IUserContext _userContext;

            public Handler(IWorkspaceRepository workspaces, IUserContext userContext)
            {
                _workspaces = workspaces;
                _userContext = userContext;
            }

            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var item = await _workspaces.AddAsync(EntityFactory.CreateQuestionnaire(command.Title, 
                    command.CriteriaNo, 
                    command.AlternativesNo,
                    _userContext));

                await _workspaces.UnitOfWork.SaveChangesAsync();

                return item.Id.Hash();
            }

            
        }
    }
}
