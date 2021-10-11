using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;

namespace Topsis.AcceptanceTests.TestServer
{
    public static class SQLiteDatabase
    {
        private const string InMemoryFileName = ":memory:";
        private const string ScenarioPlaceholder = "<scenario>";

        public static DbConnection OpenConnection(TestContext context, string inputConnectionString)
        {
            var connBuilder = new SqliteConnectionStringBuilder(inputConnectionString);

            var connectionString = GetConnectionString(context, connBuilder);

            var connection = new SqliteConnection(connectionString);
            connection.Open();
            return connection;
        }

        private static void SafeDelete(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void EnsureDirectory(string fileName)
        {
            var path = new FileInfo(fileName);
            if (path.Directory.Exists == false)
            {
                path.Directory.Create();
            }
        }

        public static string GetConnectionString(TestContext context, SqliteConnectionStringBuilder builder)
        {
            if (builder.Mode == SqliteOpenMode.Memory)
            {
                return builder.ConnectionString;
            }

            var dataSource = builder.DataSource;
            if (IsScenarioBased(dataSource))
            {
                var scenario = context.BuildScenarioFileName();
                dataSource = builder.DataSource = dataSource.Replace(ScenarioPlaceholder, $"{scenario}");
            }

            if (ShouldDelete(dataSource))
            {
                SafeDelete(dataSource);
            }

            EnsureDirectory(dataSource);
            return builder.ConnectionString;
        }

        private static List<string> Connections = new List<string>();
        private static bool ShouldDelete(string dataSource)
        {
            if (Connections.BinarySearch(dataSource) > -1)
            {
                return false;
            }

            Connections.Add(dataSource);
            return true;
        }

        private static bool IsScenarioBased(string filename)
        {
            return filename.Contains(ScenarioPlaceholder);
        }

        private static bool IsInMemory(string fileName)
        {
            return string.Equals(InMemoryFileName, fileName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
