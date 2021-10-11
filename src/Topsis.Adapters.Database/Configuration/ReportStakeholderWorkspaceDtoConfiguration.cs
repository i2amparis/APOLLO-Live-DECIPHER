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
SELECT 
    ws.Id as WorkspaceId,
    ws.Title as WorkspaceTitle,
    ws.Description as WorkspaceDescription,
	ws.CurrentStatus,
    NULL as VoteId,
    NULL as StakeholderId
from
            WsWorkspaces        ws
where
    ws.CurrentStatus in (1,2,3)
UNION
SELECT 
    ws.Id as WorkspaceId,
    ws.Title as WorkspaceTitle,
    ws.Description as WorkspaceDescription,
	ws.CurrentStatus,
    v.Id as VoteId,
    v.ApplicationUserId as StakeholderId
from
            WsWorkspaces        ws
inner join  WsStakeholderVotes  v   on ws.Id = v.WorkspaceId
";

            builder.HasNoKey().ToQuery(() =>
               _dbContext.ReportStakeholderWorkspaces.FromSqlInterpolated(sql)
               .AsQueryable());
        }
    }
}
