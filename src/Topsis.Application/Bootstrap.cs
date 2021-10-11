using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Topsis.Application
{
    public static class Bootstrap
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = typeof(Bootstrap).Assembly;
            services.AddAutoMapper(assembly);
            services.AddMediatR(assembly);

            services.AddScoped<IMessageBus, MessageBus>();

            return services;
        }
    }
}
