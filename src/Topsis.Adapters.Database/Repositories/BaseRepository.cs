using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;

namespace Topsis.Adapters.Database.Repositories
{
    public class BaseRepository<TKey, TEntity> : IRepository<TKey, TEntity> where TEntity : class
    {
        protected readonly WorkspaceDbContext _db;

        public BaseRepository(WorkspaceDbContext db)
        {
            _db = db;
        }

        public IUnitOfWork UnitOfWork => _db;

        protected DbSet<TEntity> Set => _db.Set<TEntity>();

        public async Task<TEntity> AddAsync(TEntity item)
        {
            var result = await Set.AddAsync(item);
            return result.Entity;
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await Set.FindAsync(id);
        }

        public virtual TKey GetNextIdAsync()
        {
            var keyType = typeof(TKey);
            if (keyType == typeof(string))
            {
                return (TKey)(object)EntityFactory.NewId();
            }

            if (IsNumericType(keyType))
            {
                return (TKey)(object)0;
            }

            throw new NotImplementedException($"You have to override 'GetNextIdAsync' in repository: {this.GetType().Name}");
        }

        private static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public Task<IReadOnlyList<TEntity>> ListAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IPaginatedList<TEntity>> GetPagedReponseAsync(int page, int size)
        {
            return await PaginatedList<TEntity>.CreateAsync(Set.AsQueryable(), page, size);
        }

        public async Task AddRangeAsync(params TEntity[] entities)
        {
            await Set.AddRangeAsync(entities);
        }
    }
}
