using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Threading.Tasks;
using Topsis.Adapters.Database.Configuration;
using Topsis.Adapters.Database.Seed;
using Topsis.Application;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain;

namespace Topsis.Adapters.Database
{
    public class WorkspaceDbContext : 
        IdentityDbContext<ApplicationUser, ApplicationRole, string>, 
        IUnitOfWork
    {
        private IDbContextTransaction _currentTransaction;
        private readonly IMessageBus _bus;

        public WorkspaceDbContext(DbContextOptions<WorkspaceDbContext> options, IMessageBus bus)
            : base(options)
        {
            _bus = bus;
        }

        // Reports.
        public DbSet<StakeholderWorkspaceDto> ReportStakeholderWorkspaces { get; set; }
        public DbSet<StakeholderAnswerDto> ReportStakeholderAnswers { get; set; }

        // Domain
        public DbSet<Workspace> WsWorkspaces { get; set; }
        public DbSet<WorkspaceReport> WsWorkspacesReports { get; set; }

        public DbSet<Questionnaire> WsQuestionnaires { get; set; }
        public DbSet<Criterion> WsCriteria { get; set; }
        public DbSet<Alternative> WsAlternatives { get; set; }

        public DbSet<StakeholderVote> WsStakeholderVotes { get; set; }
        public DbSet<StakeholderAnswer> WsStakeholderAnswers { get; set; }
        public DbSet<StakeholderCriterionImportance> WsStakeholderCriteriaImportance { get; set; }

        // Lookup.
        public DbSet<Country> WsCountries { get; set; }
        public DbSet<JobCategory> WsJobCategories { get; set; }

        #region [ Transaction ]
        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();

                await (_currentTransaction?.CommitAsync() ?? Task.CompletedTask);
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        } 
        #endregion

        #region [ Helpers ]
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.AddPostgreSql("server=db; port=5432; uid=root; pwd=password; database=topsis;");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new CountryConfiguration());
            builder.ApplyConfiguration(new JobCategoryConfiguration());

            builder.ApplyConfiguration(new WorkspaceConfiguration());
            builder.ApplyConfiguration(new WorkspaceReportConfiguration());
            builder.ApplyConfiguration(new QuestionnaireConfiguration());
            builder.ApplyConfiguration(new AlternativeConfiguration());
            builder.ApplyConfiguration(new CriterionConfiguration());

            builder.ApplyConfiguration(new StakeholderVoteConfiguration());
            builder.ApplyConfiguration(new StakeholderAnswerConfiguration());

            builder.Entity<ApplicationUser>(ApplicationUserOverride);

            // reports.
            builder.ApplyConfiguration(new ReportStakeholderAnswerDtoConfiguration(this));
            builder.ApplyConfiguration(new ReportStakeholderWorkspaceDtoConfiguration(this));

            SeedData(builder);
        }

        private void ApplicationUserOverride(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.OwnsOne(x => x.Created);
            builder.HasMany(x => x.Votes).WithOne();
        }

        private void SeedData(ModelBuilder builder)
        {
            // Identity.
            IdentitySeed.ApplyTo(builder);

            // Workspace.
            builder.Entity<Country>().HasData(Country.AllCountries());
            builder.Entity<JobCategory>().HasData(JobCategory.AllJobCategories());
        }
        #endregion
    }
}
