using FluentValidation;
using MediatR;
using System;
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
                RuleFor(m => m.Answers).Must(CheckCriteria)
                    .WithName("Data.Answers")
                    .WithMessage("You cannot answer all criteria alternatives with the same answer.");
                RuleForEach(model => model.Answers)
                    .SetValidator(AddAnswerValidator);
            }

            private bool CheckCriteria(List<Command.StakeholderAnswerDto> arg)
            {
                var criteria = arg.GroupBy(x => x.CriterionId).ToDictionary(x => x.ToArray());
                int countCriteriaWithSameAnswers = 0;
                foreach (var kvp in criteria)
                {
                    if (kvp.Value.All(x => x.Value == kvp.Value.First().Value))
                    {
                        countCriteriaWithSameAnswers++;
                    }
                }

                // 8/6.2022 - Konstantinos: at least one criterion should have a different answer value.
                // if for all criteria we have the same answers, 
                // example: 
                // c1: a1-1, a2-1, a3-1 -- all 1
                // c2: a1-4, a2-4, a3-4 -- all 4
                // c3: a1-1, a2-1, a3-1 -- all 1
                // c3: a1-3, a2-3, a3-3 -- all 3
                // then this is invalid
                return countCriteriaWithSameAnswers != criteria.Count();
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
            private readonly IVoteRepository _votes;

            public Handler(IUserContext userContext, IVoteRepository votes)
            {
                _userContext = userContext;
                _votes = votes;
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
                await _votes.AddAsync(vote);
                await _votes.UnitOfWork.SaveChangesAsync();

                return vote.Id.Hash();    
            }
        }
    }
}
