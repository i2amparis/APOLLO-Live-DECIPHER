using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Topsis.Adapters.Encryption
{
    public class DatabaseFactory : IDesignTimeDbContextFactory<DataProtectionKeysContext>
    {
        public DataProtectionKeysContext CreateDbContext(string[] args)
        {
            var deployConnectionString = Environment.GetEnvironmentVariable("DeployConnectionString")
                ?? "Host=127.0.0.1;Port=5432;Username=dbuser;Password=password;Database=topsis-db";

            var optionsBuilder = new DbContextOptionsBuilder<DataProtectionKeysContext>();
            SetupDatabase(optionsBuilder, deployConnectionString);

            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            if (isDevelopment)
            {
                optionsBuilder.EnableDetailedErrors().EnableSensitiveDataLogging();
            }

            return new DataProtectionKeysContext(optionsBuilder.Options);
        }

        public static DbContextOptionsBuilder SetupDatabase(DbContextOptionsBuilder builder,
            string deployConnectionString)
        {
            return builder.UseNpgsql(deployConnectionString,
                            b => b.MigrationsHistoryTable(DataProtectionKeysContext.MigrationsHistoryTable));
        }
    }

    public class DataProtectionKeysContext : DbContext, IDataProtectionKeyContext
    {
        // A recommended constructor overload when using EF Core 
        // with dependency injection.
        public DataProtectionKeysContext(DbContextOptions<DataProtectionKeysContext> options)
            : base(options) { }

        public const string MigrationsHistoryTable = "__KeysMigrationsHistory";

        // This maps to the table that stores keys.
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
    }
}
