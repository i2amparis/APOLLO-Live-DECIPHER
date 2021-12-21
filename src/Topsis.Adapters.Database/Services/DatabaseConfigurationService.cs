using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using Topsis.Application.Contracts.Database;

namespace Topsis.Adapters.Database.Services
{
    public class DatabaseConfigurationService : IDatabaseService
    {
        private readonly DatabaseSettings _config;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<DatabaseConfigurationService> _logger;
        private string _migrationConnection;
        private string _userConnection;

        public DatabaseConfigurationService(IOptions<DatabaseSettings> options,
            IHostEnvironment environment,
            ILogger<DatabaseConfigurationService> logger)
        {
            _config = options.Value;
            DatabaseSettings.Validate(_config);

            _environment = environment;
            _logger = logger;
        }

        public string GetMigrationConnectionString()
        {
            _logger.LogDebug($"Getting migration connection (env:{_environment.EnvironmentName}).");
            if (string.IsNullOrWhiteSpace(_migrationConnection) == false)
            {
                return _migrationConnection;
            }

            _migrationConnection = GetConnectionString(_config.Migration);
            return _migrationConnection;
        }

        public string GetRuntimeConnectionString()
        {
            _logger.LogDebug($"Getting user connection (env:{_environment.EnvironmentName}).");
            if (string.IsNullOrWhiteSpace(_userConnection) == false)
            {
                return _userConnection;
            }

            _userConnection = GetConnectionString(_config.Runtime);
            return _userConnection;
        }

        private string GetConnectionString(DatabaseUserSettings u)
        {
            var connectionString = $"Server={_config.Server}; port={_config.Port}; uid={u.UserId}; pwd={u.Password}; database={_config.DatabaseName};";
            if (string.IsNullOrWhiteSpace(_config.InstanceName))
            {
                return connectionString;
            }

            _logger.LogDebug($"Building connection ({_environment.EnvironmentName}:{_config.InstanceName}).");
            var dbSocketDir = Environment.GetEnvironmentVariable("DB_SOCKET_PATH") ?? "/cloudsql";

            var builder = new MySqlConnectionStringBuilder(connectionString)
            {
                // The Cloud SQL proxy provides encryption between the proxy and instance.
                SslMode = MySqlSslMode.None,
                // Remember - storing secrets in plain text is potentially unsafe. Consider using
                // something like https://cloud.google.com/secret-manager/docs/overview to help keep
                // secrets secret.
                ConnectionProtocol = MySqlConnectionProtocol.UnixSocket
            };
            builder.Pooling = true;
            builder.Server = $"{dbSocketDir}/{_config.InstanceName}";

            return builder.ConnectionString;
        }
    }
}
