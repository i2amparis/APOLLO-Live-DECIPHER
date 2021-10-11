using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Configuration
{
    public class StakeholderAnswerConfiguration : IEntityTypeConfiguration<StakeholderAnswer>
    {
        public void Configure(EntityTypeBuilder<StakeholderAnswer> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
