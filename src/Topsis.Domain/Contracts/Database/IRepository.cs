using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Domain;

namespace Topsis.Application.Contracts.Database
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public interface IPaginatedList<T> : IReadOnlyList<T>
    {
        int PageNumber { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }

        bool HasNextPage { get; }
    }

    public interface IRepository<TKey, TEntity> where TEntity : class
    {
        IUnitOfWork UnitOfWork { get; }
        Task<TEntity> GetByIdAsync(TKey id);
        Task<IReadOnlyList<TEntity>> ListAllAsync();
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<IPaginatedList<TEntity>> GetPagedReponseAsync(int page, int size);
    }

    public interface IGuidRepository<TEntity> : IRepository<Guid, TEntity> where TEntity : class
    { 
    }

    public interface IIntRepository<TEntity> : IRepository<int, TEntity> where TEntity : class
    {
    }

    public interface IWorkspaceRepository : IIntRepository<Workspace>
    {
        Task<Workspace> GetByIdForCalculationAsync(int id);
        Task ClearVotesAndReportsAsync(int id);
        Task<Workspace> FindImportedAsync(string importKey);
    }

    public interface IVoteRepository : IIntRepository<StakeholderVote>
    {
    }

    public interface IApplicationUserRepository<TEntity> : IRepository<string, TEntity> 
        where TEntity : class
    {
        Task AddUserToRoleAsync(string userId, string roleName);
    }

    public interface IWorkspaceReportRepository : IIntRepository<WorkspaceReport>
    {
        Task<WorkspaceReport> FindAsync(int workspaceId, AlgorithmType algorithm);
    }
}
