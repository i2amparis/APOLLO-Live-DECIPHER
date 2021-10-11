using Microsoft.Extensions.DependencyInjection;
using Topsis.Adapters.Algorithm.Queue;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Results;

namespace Topsis.Adapters.Algorithm
{
    public static class Bootstrap
    {
        public static IServiceCollection AddAlgorithm(this IServiceCollection services)
        {
            services.AddSingleton<ITopsisAlgorithm, TopsisAnalyzer>();
            services.AddSingleton<ICalculateResultsProducer, Producer>();
            services.AddSingleton<ICalculateResultsProcessor, Processor>();

            services.AddHostedService<Consumer>();
            return services;
        }
    }
}
