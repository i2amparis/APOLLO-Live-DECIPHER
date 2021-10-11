using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;

namespace Topsis.Adapters.Database
{
    public class PaginatedList<T> : List<T>, IPaginatedList<T>
    {
        public int PageNumber { get; }
        public int TotalPages { get; }

        public PaginatedList(List<T> items, int count, int page, int pageSize)
        {
            PageNumber = page;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int page, int pageSize)
        {
             if (page < 1)
                page = 1;

            var count = await source.CountAsync();
            var query = source.Skip((page - 1) * pageSize).Take(pageSize);
            var items = await query.ToListAsync();
            return new PaginatedList<T>(items, count, page, pageSize);
        }
    }
}
