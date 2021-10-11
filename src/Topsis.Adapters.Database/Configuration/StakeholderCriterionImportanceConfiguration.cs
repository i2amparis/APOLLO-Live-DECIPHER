using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Configuration
{
    public class StakeholderCriterionImportanceConfiguration : IEntityTypeConfiguration<StakeholderCriterionImportance>
    {
        public void Configure(EntityTypeBuilder<StakeholderCriterionImportance> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
