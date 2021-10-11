using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;
using Topsis.Domain.Common;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Features
{
    public static class Vote
    {
        public class Command : IRequest<string>
        {
            public string Id { get; set; }
            public List<StakeholderAnswerDto> Answers { get; set; }
            public Dictionary<int, int> CriteriaImportance { get; set; }

            public class StakeholderAnswerDto
            { 
                [Required]
                public int CriterionId { get; set; }
                [Required]
                public int AlternativeId { get; set; }
                [Required]
                public double Value { get; set; }

                public StakeholderAnswer ToDomainModel()
                {
                    return new StakeholderAnswer
                    {
                        AlternativeId = AlternativeId,
                        CriterionId = CriterionId,
                        Value = Value
                    };
                }
            }
        }

        public class VoteCommandValidator : AbstractValidator<Command>
        {
            public VoteCommandValidator()
            {
                RuleFor(m => m.Id).NotNull();
                RuleFor(m => m.Answers).NotEmpty();
                RuleForEach(model => model.Answers)
                    .SetValidator(AddAnswerValidator);
            }

            private IValidator<Command.StakeholderAnswerDto> AddAnswerValidator(Command command, 
                Command.StakeholderAnswerDto answer)
            {
                return new ChildValidator();
            }

            public class ChildValidator : AbstractValidator<Command.StakeholderAnswerDto>
            {
                public ChildValidator()
                {
                    this.RuleFor(model => model.Value).NotNull();
                }
            }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly IUserContext _userContext;
            private readonly IVoteRepository _repository;

            public Handler(IUserContext userContext, IVoteRepository repository)
            {
                _userContext = userContext;
                _repository = repository;
            }

            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var workspaceId = command.Id.DehashInts().FirstOrDefault();

                var vote = new StakeholderVote 
                { 
                    WorkspaceId = workspaceId, 
                    ApplicationUserId = _userContext.UserId 
                };
                
                vote.Accept(command.Answers.Select(x => x.ToDomainModel()).ToArray(), command.CriteriaImportance);
                await _repository.AddAsync(vote);
                await _repository.UnitOfWork.SaveChangesAsync();

                return vote.Id.Hash();    
            }
        }
    }
}
