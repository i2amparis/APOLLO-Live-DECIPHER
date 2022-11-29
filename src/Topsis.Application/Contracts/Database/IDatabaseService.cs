using System;

namespace Topsis.Application.Contracts.Database
{
    public interface IDatabaseService
    {
        string GetRuntimeConnectionString();
        string GetMigrationConnectionString();
        string GetDatabaseEngine();
    }

    public class DatabaseSettings
    {
        public const string ENGINE_POSTGRESQL = "postgresql";

        public DatabaseSettings()
        {
            Engine = ENGINE_POSTGRESQL;
            DatabaseName = "topsis-web";
            Server = "127.0.0.1";
            Port = 3306;
            Migration = new DatabaseUserSettings();
            Runtime = new DatabaseUserSettings();
        }

        public string Engine { get; set; }
        public string DatabaseName { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string InstanceName { get; set; }
        public DatabaseUserSettings Migration { get; set; }
        public DatabaseUserSettings Runtime { get; private set; }

        public static void Validate(DatabaseSettings config)
        {
            config = config ?? throw new ArgumentNullException(nameof(config));
            CheckEmpty(config.DatabaseName, nameof(config.DatabaseName));
            CheckEmpty(config.Server, nameof(config.Server));

            var user = config.Runtime ?? throw new ArgumentNullException(nameof(config));
            CheckEmpty(user.UserId, nameof(user.UserId));
            CheckEmpty(user.Password, nameof(user.Password));

            var migration = config.Migration ?? throw new ArgumentNullException(nameof(config));
            CheckEmpty(migration.UserId, nameof(migration.UserId));
            CheckEmpty(migration.Password, nameof(migration.Password));
        }

        private static void CheckEmpty(string value, string property)
        {
            if (string.IsNullOrWhiteSpace(value))
            {

                throw new ArgumentException($"Database 'property:{property}' cannot be empty.");
            }
        }
    }

    public class DatabaseUserSettings
    {
        public DatabaseUserSettings()
        {
            UserId = "root";
            Password = "password";
        }

        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
