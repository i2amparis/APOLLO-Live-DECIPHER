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
    latest.""WorkspaceId"",
    latest.""ApplicationUserId"" AS ""StakeholderId"",
    u.""JobCategoryId"",
    latest.""VoteId"",
	sa.""AlternativeId"",
	sa.""CriterionId"", 
    sa.""Value"" AS ""AnswerValue"",
    im.""Weight"" AS ""CriterionWeight""
from
            ""WsStakeholderAnswers""                sa
inner JOIN  ""WsCriteria""                          c   on sa.""CriterionId"" = C.""Id""
INNER JOIN  (select MAX(v.""Id"") AS ""VoteId"", v.""WorkspaceId"", v.""ApplicationUserId""
				FROM ""WsStakeholderVotes"" v
				GROUP BY v.""WorkspaceId"", v.""ApplicationUserId"")				latest	 ON sa.""VoteId"" = latest.""VoteId""
INNER JOIN  ""AspNetUsers""                         u   on latest.""ApplicationUserId"" = u.""Id""
inner JOIN  ""WsStakeholderCriteriaImportance""     im  on sa.""VoteId"" = im.""VoteId"" and C.""Id"" = im.""CriterionId""
";

            builder.HasNoKey().ToQuery(() =>
               _dbContext.ReportStakeholderAnswers.FromSqlInterpolated(sql).AsQueryable());
        }
    }
}
