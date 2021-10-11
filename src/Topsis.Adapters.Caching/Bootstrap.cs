using Microsoft.Extensions.DependencyInjection;
using Topsis.Application.Contracts.Caching;

namespace Topsis.Adapters.Caching
{
    public static class Bootstrap
    {
        public static IServiceCollection AddCachingServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICachingService, CachingService>();

            return services;
        }
    }
}
