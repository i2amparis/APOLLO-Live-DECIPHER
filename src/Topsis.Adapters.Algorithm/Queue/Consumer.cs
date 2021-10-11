using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Results;

namespace Topsis.Adapters.Algorithm.Queue
{

    public class Consumer : BackgroundService, ICalculateResultsConsumer
    {
        private readonly ICalculateResultsProducer _producer;
        private readonly ICalculateResultsProcessor _processor;
        private readonly ILogger<Consumer> _logger;

        public Consumer(
            ICalculateResultsProducer producer,
            ICalculateResultsProcessor processor,
            ILogger<Consumer> logger)
        {
            _producer = producer;
            _processor = processor;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consuming inbox messages background service started.");

            await foreach (var reportKey in _producer.GetNextCalculationAsync(stoppingToken))
            {
                await _processor.ProcessAsync(reportKey);
            }
        }
    }
}
