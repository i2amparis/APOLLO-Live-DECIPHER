using Microsoft.Extensions.DependencyInjection;
using Topsis.Application.Contracts.Import;

namespace Topsis.Adapters.Import
{
    public static class Bootstrap
    {
        public static IServiceCollection AddImportServices(this IServiceCollection services)
        {
            services.AddScoped<IImportService, ImportService>();

            return services;
        }
    }
}
