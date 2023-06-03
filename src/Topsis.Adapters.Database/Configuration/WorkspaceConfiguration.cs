using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Configuration
{
    public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
    {
        public void Configure(EntityTypeBuilder<Workspace> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.Parent).WithOne();
            builder.HasMany(x => x.Votes).WithOne(x => x.Workspace);
            builder.HasMany(x => x.Reports).WithOne(x => x.Workspace);

            builder.Property("_settings").HasColumnName("SettingsJson");
        }
    }
}
