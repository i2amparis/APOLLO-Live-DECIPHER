using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Topsis.Adapters.Database.Services
{
    public class MigrationHostedService : BackgroundService
    {
        // We need to inject the IServiceProvider so we can create 
        // the scoped service, MyDbContext
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MigrationHostedService> _logger;

        public MigrationHostedService(IServiceProvider serviceProvider, ILogger<MigrationHostedService> logger)
        {
            _serviceProvider = serviceProvider;
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
                    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var connectionString = configuration.GetConnectionString("MigrationConnection");
                    var builder = new DbContextOptionsBuilder<WorkspaceDbContext>();
                    builder.AddMariaDbOptions(connectionString);

                    // Migrate.
                    var myDbContext = new WorkspaceDbContext(builder.Options, null);
                    await myDbContext.Database.MigrateAsync();
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
