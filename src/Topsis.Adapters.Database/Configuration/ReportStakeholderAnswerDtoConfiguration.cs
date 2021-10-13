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
    latest.WorkspaceId,
    latest.ApplicationUserId as StakeholderId,
    u.JobCategoryId,
    latest.VoteId,
	sa.AlternativeId,
	sa.CriterionId, 
    sa.Value as AnswerValue,
    im.Weight as CriterionWeight
from
            WsStakeholderAnswers                sa
inner join  WsCriteria                          c   on sa.CriterionId = c.Id
INNER JOIN  (select MAX(v.Id) AS VoteId, v.WorkspaceId, v.ApplicationUserId
				from WsStakeholderVotes v
				GROUP BY v.WorkspaceId, v.ApplicationUserId)				latest	 ON sa.VoteId = latest.VoteId
INNER JOIN  AspNetUsers                         u   on latest.ApplicationUserId = u.Id
inner join  WsStakeholderCriteriaImportance     im  on sa.VoteId = im.VoteId and c.Id = im.CriterionId
";

            builder.HasNoKey().ToQuery(() =>
               _dbContext.ReportStakeholderAnswers.FromSqlInterpolated(sql).AsQueryable());
        }
    }
}
