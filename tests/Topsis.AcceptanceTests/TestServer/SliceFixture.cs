using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Respawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topsis.AcceptanceTests.Fakes;
using Topsis.Adapters.Algorithm.Queue;
using Topsis.Adapters.Database;
using Topsis.Application.Contracts.Results;
using Topsis.Domain;
using Topsis.Domain.Contracts;
using Topsis.Web;
using Xunit;

namespace Topsis.AcceptanceTests.TestServer
{

    [CollectionDefinition(nameof(SliceFixture))]
    public class SliceFixtureCollection : ICollectionFixture<SliceFixture> { }

    public class SliceFixture : IAsyncLifetime
    {
        private readonly Checkpoint _checkpoint;

        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ContosoTestApplicationFactory _factory;

        public SliceFixture()
        {
            _factory = new ContosoTestApplicationFactory();

            _configuration = _factory.Services.GetRequiredService<IConfiguration>();
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

            _checkpoint = new Checkpoint()
            {
                SchemasToInclude = new[]
                {
                    "public"
                },
                TablesToIgnore = new[] { 
                    "AspNetRoles", 
                    "WsCountries", 
                    "WsJobCategories"
                }
                DbAdapter = DbAdapter.Postgres
            };
        }

        public async Task InitializeAsync()
        {
            using (var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await conn.OpenAsync();
                await _checkpoint.Reset(conn);
            }
        }

        internal void SetContext<TTest>(Scenario scenario, string userId = "test-user-id")
        {
            _factory.Context = new TestContext(typeof(TTest), scenario, userId);
        }

        internal void SwitchUser(string userId)
        {
            _factory.Context.UserContext.SwitchUser(userId);
        }

        //public ITestOutputHelper Output { get; internal set; }

        public class ContosoTestApplicationFactory
            : WebApplicationFactory<Startup>
        {
            public ContosoTestApplicationFactory()
            {
            }

            protected override IHost CreateHost(IHostBuilder builder)
            {
                return base.CreateHost(builder).MigrateDatabase();
            }

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"ConnectionStrings:DefaultConnection", _connection}
                    });
                });

                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IUserContext>(s => Context.UserContext);

                    RemoveServicesFromIoC<ICalculateResultsProducer>(services);
                    services.AddScoped<ICalculateResultsProducer, InProcessCalculateResultsProducer>();
                    
                    RemoveServicesFromIoC<IHostedService>(services);
                    //services.AddScoped<ICalculateResultsConsumer, DummyCalculateResultsConsumer>();

                    RemoveServicesFromIoC<ICalculateResultsProcessor>(services);
                    services.AddScoped<ICalculateResultsProcessor, InProcessCalculateProcessor>();

                    services.AddDbContext<WorkspaceDbContext>(options =>
                        options.UseNpgsql(_connection));
                });
            }

            private readonly string _connection =
                "Server=127.0.0.1; port=5432; User Id=dbuser; pwd=password; database=topsis-test;";
            // "Server=.\\SQLExpress;Database=Topsis-Test;Trusted_Connection=True;MultipleActiveResultSets=true";

            public TestContext Context { get; set; }
            
            private static void RemoveServicesFromIoC(Type type, IServiceCollection services)
            {
                var serviceDescriptors = services.Where(descriptor => descriptor.ServiceType == type).ToArray();
                foreach (var service in serviceDescriptors)
                {
                    var t = services.Remove(service);
                }
            }

            private static void RemoveServicesFromIoC<TService>(IServiceCollection services)
            {
                RemoveServicesFromIoC(typeof(TService), services);
            }
        }

        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkspaceDbContext>();

            try
            {
                await dbContext.BeginTransactionAsync();

                await action(scope.ServiceProvider);

                await dbContext.CommitTransactionAsync();
            }
            catch (Exception)
            {
                dbContext.RollbackTransaction();
                throw;
            }
        }

        public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkspaceDbContext>();

            try
            {
                await dbContext.BeginTransactionAsync();

                var result = await action(scope.ServiceProvider);

                await dbContext.CommitTransactionAsync();

                return result;
            }
            catch (Exception ex)
            {
                dbContext.RollbackTransaction();
                throw;
            }
        }

        public Task ExecuteDbContextAsync(Func<WorkspaceDbContext, Task> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<WorkspaceDbContext>()));

        public Task ExecuteDbContextAsync(Func<WorkspaceDbContext, ValueTask> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<WorkspaceDbContext>()).AsTask());

        public Task ExecuteDbContextAsync(Func<WorkspaceDbContext, IMediator, Task> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<WorkspaceDbContext>(), sp.GetService<IMediator>()));

        public Task<T> ExecuteDbContextAsync<T>(Func<WorkspaceDbContext, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<WorkspaceDbContext>()));

        public Task<T> ExecuteDbContextAsync<T>(Func<WorkspaceDbContext, ValueTask<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<WorkspaceDbContext>()).AsTask());

        public Task<T> ExecuteDbContextAsync<T>(Func<WorkspaceDbContext, IMediator, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<WorkspaceDbContext>(), sp.GetService<IMediator>()));

        public Task InsertAsync<T>(params T[] entities) where T : class
        {
            return ExecuteDbContextAsync(db =>
            {
                foreach (var entity in entities)
                {
                    db.Set<T>().Add(entity);
                }
                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);

                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity, TEntity2>(TEntity entity, TEntity2 entity2)
            where TEntity : class
            where TEntity2 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);

                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity, TEntity2, TEntity3>(TEntity entity, TEntity2 entity2, TEntity3 entity3)
            where TEntity : class
            where TEntity2 : class
            where TEntity3 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);
                db.Set<TEntity3>().Add(entity3);

                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2, TEntity3 entity3, TEntity4 entity4)
            where TEntity : class
            where TEntity2 : class
            where TEntity3 : class
            where TEntity4 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);
                db.Set<TEntity3>().Add(entity3);
                db.Set<TEntity4>().Add(entity4);

                return db.SaveChangesAsync();
            });
        }

        public Task<TEntity> FindAsync<TKey, TEntity>(TKey id)
            where TEntity : class, IEntity<TKey>
        {
            return ExecuteDbContextAsync(db => db.Set<TEntity>().FindAsync(id).AsTask());
        }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(request);
            });
        }

        public Task SendAsync(IRequest request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(request);
            });
        }

        public Task DisposeAsync()
        {
            _factory?.Dispose();
            return Task.CompletedTask;
        }
    }
}
