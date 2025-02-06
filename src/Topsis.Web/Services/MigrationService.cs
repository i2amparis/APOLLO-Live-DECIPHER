using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Topsis.Adapters.Database;
using Topsis.Adapters.Database.Seed;
using Topsis.Adapters.Encryption;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain;

namespace Topsis.Web.Services;

public interface IMigrationService
{
    Task EnsureMigrationAsync();
}

public class MigrationService : IMigrationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<MigrationService> _logger;

    public MigrationService(IServiceProvider serviceProvider, IDatabaseService databaseService, ILogger<MigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _databaseService = databaseService;
        _logger = logger;
    }

    public async Task EnsureMigrationAsync()
    {
        await MigrateAppDatabaseAsync();
        await MigrateEncryptionDatabaseAsync();
        await InitializeDatabaseAsync();
    }

    #region [ Helpers ]
    private async Task MigrateAppDatabaseAsync()
    {
        _logger.LogDebug("Starting app database migration.");

        // Create a new scope to retrieve scoped services
        using (var scope = _serviceProvider.CreateScope())
        {
            try
            {
                var db = BuildAppDbContext();
                await db.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        _logger.LogDebug("Finished app database migration.");
    }

    private WorkspaceDbContext BuildAppDbContext()
    {
        // db options.
        var builder = new DbContextOptionsBuilder<WorkspaceDbContext>();
        builder.SetupDatabase(_databaseService.GetDatabaseEngine(), _databaseService.GetMigrationConnectionString());
        return new WorkspaceDbContext(builder.Options, null);
    }

    private async Task MigrateEncryptionDatabaseAsync()
    {
        _logger.LogDebug("Starting encryption database migration.");

        // Create a new scope to retrieve scoped services
        using (var scope = _serviceProvider.CreateScope())
        {
            try
            {
                // Db Options.
                var builder = new DbContextOptionsBuilder<DataProtectionKeysContext>();
                Adapters.Encryption.DatabaseFactory.SetupDatabase(builder, _databaseService.GetMigrationConnectionString());

                // Migrate.
                var db = new DataProtectionKeysContext(builder.Options);
                await db.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        _logger.LogDebug("Finished encryption database migration.");
    }

    private async Task InitializeDatabaseAsync()
    {
        _logger.LogDebug("Starting db initialization.");
        using (var scope = _serviceProvider.CreateScope())
        {
            try
            {
                var db = BuildAppDbContext();
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
    #endregion
}
