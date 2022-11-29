using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using System;
using Topsis.Application.Contracts.Database;

namespace Topsis.Adapters.Database
{
    public class DatabaseFactory : IDesignTimeDbContextFactory<WorkspaceDbContext>
    {
        public WorkspaceDbContext CreateDbContext(string[] args)
        {
            var databaseEngine = Environment.GetEnvironmentVariable("DatabaseSettings__Engine") ?? DatabaseSettings.ENGINE_POSTGRESQL;
            var deployConnectionString = Environment.GetEnvironmentVariable("DeployConnectionString")
                ?? "Host=127.0.0.1;Port=5432;Username=dbuser;Password=password;Database=topsis-db";

            var optionsBuilder = new DbContextOptionsBuilder<WorkspaceDbContext>();
            optionsBuilder.Setup(databaseEngine, deployConnectionString);

            return new WorkspaceDbContext(optionsBuilder.Options, null);
        }
    }
}
