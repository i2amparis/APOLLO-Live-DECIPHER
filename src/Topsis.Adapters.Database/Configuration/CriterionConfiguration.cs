using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Configuration
{
    public class CriterionConfiguration : IEntityTypeConfiguration<Criterion>
    {
        public void Configure(EntityTypeBuilder<Criterion> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
       }
    }
}
