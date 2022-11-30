using Microsoft.Extensions.DependencyInjection;
using Topsis.Application.Contracts.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Topsis.Adapters.Encryption.Services
{
    public class MigrationHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<MigrationHostedService> _logger;

        public MigrationHostedService(IServiceProvider serviceProvider,
            IDatabaseService databaseService,
            ILogger<MigrationHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _databaseService = databaseService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Starting migration.");

            // Create a new scope to retrieve scoped services
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    // Db Options.
                    var builder = new DbContextOptionsBuilder<DataProtectionKeysContext>();
                    DatabaseFactory.SetupDatabase(builder, _databaseService.GetMigrationConnectionString());

                    // Migrate.
                    var db = new DataProtectionKeysContext(builder.Options);
                    db.Database.Migrate();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }

            _logger.LogDebug("Finished migration.");
        }
    }
}
