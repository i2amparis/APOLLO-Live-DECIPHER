using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Configuration
{

    public class StakeholderVoteConfiguration : IEntityTypeConfiguration<StakeholderVote>
    {
        public void Configure(EntityTypeBuilder<StakeholderVote> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Answers).WithOne(x => x.Vote);
            builder.HasMany(x => x.CriteriaImportance).WithOne(x => x.Vote);

            builder.HasIndex(p => p.WorkspaceId)
                .IsUnique(false)
                .HasDatabaseName("IX_STAKEHOLDERVOTE_WORKSPACEID");
        }
    }
}
