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
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<WorkspaceDbContext>(builder => builder.AddMariaDbOptions(connectionString));

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

    public static class MariaDbOptions
    {
        public static void AddMariaDbOptions(this DbContextOptionsBuilder builder, string connection)
        {
            var serverVersion = new ServerVersion(new Version(10, 5, 1));
            builder.UseMySql(connection, options => options.ServerVersion(serverVersion));
        }
    }
}
