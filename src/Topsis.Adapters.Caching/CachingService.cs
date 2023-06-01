using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Caching;

namespace Topsis.Adapters.Caching
{
    public class CachingService : ICachingService
    {
        private readonly IMemoryCache _cache;

        public CachingService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<TItem> GetOrCreateAsync<TItem>(string key, 
            Func<Task<TItem>> factory, 
            TimeSpan? slidingExpiration = null)
        {
            return await _cache.GetOrCreateAsync(key,
                async entry => {
                    if (slidingExpiration.HasValue)
                    {
                        entry.SetSlidingExpiration(slidingExpiration.Value); 
                    }

                    var item = await factory();
                    return item;
                });
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Set(string key, object value, TimeSpan? slidingExpiration = null)
        {
            if (slidingExpiration.HasValue)
            {
                _cache.Set(key, value, absoluteExpirationRelativeToNow: slidingExpiration.Value);
                return;
            }

            _cache.Set(key, value);
        }

        public object Get(string key)
        { 
            return _cache.Get(key);
        }
    }
}
