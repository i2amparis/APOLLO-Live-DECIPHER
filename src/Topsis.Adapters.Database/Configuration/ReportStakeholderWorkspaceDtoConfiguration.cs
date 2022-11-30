using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Configuration
{
    internal class ReportStakeholderWorkspaceDtoConfiguration : IEntityTypeConfiguration<StakeholderWorkspaceDto>
    {
        // https://github.com/dotnet/efcore/issues/18957

        private readonly WorkspaceDbContext _dbContext;

        public ReportStakeholderWorkspaceDtoConfiguration(WorkspaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Configure(EntityTypeBuilder<StakeholderWorkspaceDto> builder)
        {
            
            FormattableString sql = $@"
SELECT r.""CurrentStatus"", r.""StakeholderId"", r.""VoteId"", r.""WorkspaceDescription"", r.""WorkspaceId"", r.""WorkspaceTitle""
      FROM (
      
          SELECT 
          	  ws.""Id"" AS ""WorkspaceId"",
              ws.""Title"" AS ""WorkspaceTitle"",
              ws.""Description"" AS ""WorkspaceDescription"",
                ws.""CurrentStatus"",
              NULL AS ""VoteId"",
              NULL AS ""StakeholderId""
          from
                      ""WsWorkspaces"" as        ws
          where
              ws.""CurrentStatus"" in (1,2,3)
          UNION
          SELECT 
          	  ws.""Id"" AS ""WorkspaceId"",
              ws.""Title"" AS ""WorkspaceTitle"",
              ws.""Description"" AS ""WorkspaceDescription"",
                ws.""CurrentStatus"",
              v.""Id"" AS ""VoteId"",
              v.""ApplicationUserId"" AS ""StakeholderId""
          from
                   ""WsWorkspaces""        ws
          inner JOIN  ""WsStakeholderVotes""  v   on ws.""Id"" = v.""WorkspaceId""
      ) AS r
      WHERE (r.""StakeholderId"" IS NULL)
";

            builder.HasNoKey().ToQuery(() =>
               _dbContext.ReportStakeholderWorkspaces.FromSqlInterpolated(sql)
               .AsQueryable());
        }
    }
}
