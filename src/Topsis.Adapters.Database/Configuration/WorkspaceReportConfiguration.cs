using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Configuration
{
    public class WorkspaceReportConfiguration : IEntityTypeConfiguration<WorkspaceReport>
    {
        public void Configure(EntityTypeBuilder<WorkspaceReport> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property("_analysisResultJson").HasColumnName("AnalysisResultJson");

            builder.HasIndex(x => new { x.WorkspaceId, x.Round }).IsUnique().HasDatabaseName("UIX_WorkspaceId_Round");

            builder.Property(x => x.Round).HasDefaultValue(FeedbackRound.First);
        }
    }
}
