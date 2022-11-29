using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;
using Topsis.Adapters.Database.Identity;
using Topsis.Adapters.Database.Reports;
using Topsis.Adapters.Database.Repositories;
using Topsis.Adapters.Database.Services;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain.Contracts;

namespace Topsis.Adapters.Database
{
    public static class Bootstrap
    {
        // Mariadb in docker: https://mariadb.com/kb/en/installing-and-using-mariadb-via-docker/

        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));

            services.AddSingleton<IDatabaseService, DatabaseConfigurationService>();
            services.AddDbContext<WorkspaceDbContext>((serviceProvider, dbContextBuilder) =>
            {
                var service = serviceProvider.GetRequiredService<IDatabaseService>();
                dbContextBuilder.AddPostgreSql(service.GetRuntimeConnectionString());
            });

            services.AddScoped<IUserContext, UserContext>();

            services.AddScoped<IApplicationUserRepository<ApplicationUser>, ApplicationUserRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
            services.AddScoped<IWorkspaceReportRepository, WorkspaceReportRepository>();

            services.AddScoped<IReportService, ReportService>();

            services.AddHostedService<MigrationHostedService>();

            return services;
        }

        
    }

    public static class DatabaseOptions
    {
        public static void AddMariaDbOptions(this DbContextOptionsBuilder builder, string connection)
        {
            var serverVersion = new ServerVersion(new Version(10, 5, 1), ServerType.MariaDb);
            builder.UseMySql(connection, options => options.ServerVersion(serverVersion));
        }

        public static void AddMySqlDbOptions(this DbContextOptionsBuilder builder, string connection)
        {
            var serverVersion = new ServerVersion(new Version(5, 7, 34), ServerType.MySql);
            builder.UseMySql(connection, options => options.ServerVersion(serverVersion));
        }

        public static void AddPostgreSql(this DbContextOptionsBuilder builder, string connection)
        {
            builder.UseNpgsql(connection);
        }

        public static void Setup(this DbContextOptionsBuilder optionsBuilder, string engine, string connection)
        {
            switch (engine)
            {
                case "mysql":
                    optionsBuilder.AddMySqlDbOptions(connection);
                    break;
                case "mariadb":
                    optionsBuilder.AddMariaDbOptions(connection);
                    break;
                case "postgresql":
                    optionsBuilder.AddPostgreSql(connection);
                    break;
                default:
                    throw new InvalidOperationException("Set 'DatabaseSettings__Engine', use one of (mysql|mariadb|postgresql)");
            }
        }
    }
}
