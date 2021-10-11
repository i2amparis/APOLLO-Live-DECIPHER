using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using Topsis.Application.Contracts.Algorithm;

namespace Topsis.Adapters.Database.Configuration
{
    internal class ReportStakeholderAnswerDtoConfiguration : IEntityTypeConfiguration<StakeholderAnswerDto>
    {
        // https://github.com/dotnet/efcore/issues/18957

        private readonly WorkspaceDbContext _dbContext;

        public ReportStakeholderAnswerDtoConfiguration(WorkspaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Configure(EntityTypeBuilder<StakeholderAnswerDto> builder)
        {
            FormattableString sql = $@"
SELECT 
    v.WorkspaceId,
    v.ApplicationUserId as StakeholderId,
	sa.AlternativeId,
	sa.CriterionId, 
    sa.Value as AnswerValue,
    im.Weight as CriterionWeight
from
            WsStakeholderAnswers                sa
inner join  WsCriteria                          c   on sa.CriterionId = c.Id
inner join  WsStakeholderVotes                  v   on sa.VoteId = v.Id
inner join  WsStakeholderCriteriaImportance     im  on sa.VoteId = im.VoteId and c.Id = im.CriterionId
";

            builder.HasNoKey().ToQuery(() =>
               _dbContext.ReportStakeholderAnswers.FromSqlInterpolated(sql).AsQueryable());
        }
    }
}
