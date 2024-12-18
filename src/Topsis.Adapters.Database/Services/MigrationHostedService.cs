using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Adapters.Database.Seed;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Services
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
            await MigrateAsync();
            await InitializeDatabaseAsync();
        }

        private async Task MigrateAsync()
        {
            _logger.LogDebug("Starting migration.");

            // Create a new scope to retrieve scoped services
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var db = BuildContext();
                    await db.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
            _logger.LogDebug("Finished migration.");
        }

        private async Task InitializeDatabaseAsync()
        {
            _logger.LogDebug("Starting db initialization.");
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var db = BuildContext();
                    if (await db.IsInitializedAsync() == false)
                    {
                        var settings = scope.ServiceProvider.GetRequiredService<IOptions<AdminSettings>>().Value;

                        var admin = new UserCredentials
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = settings.Email,
                            Password = settings.GetPassword()
                        };
                        await db.InitializeAsync(admin);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }

            _logger.LogDebug("Finished db initialization.");
        }

        private WorkspaceDbContext BuildContext()
        {
            // Db Options.
            var builder = new DbContextOptionsBuilder<WorkspaceDbContext>();
            builder.SetupDatabase(_databaseService.GetDatabaseEngine(), _databaseService.GetMigrationConnectionString());

            // Migrate.
            var myDbContext = new WorkspaceDbContext(builder.Options, null);
            return myDbContext;
        }
    }
}
