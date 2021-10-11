using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Results;

namespace Topsis.Adapters.Algorithm.Queue
{
    public class Producer : ICalculateResultsProducer
    {
        private readonly Channel<WorkspaceReportKey> _channel;
        private readonly ILogger<Producer> _logger;

        public Producer(ILogger<Producer> logger)
        {
            _channel = Channel.CreateUnbounded<WorkspaceReportKey>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
            _logger = logger;
        }

        public void AddCalculationInQueue(WorkspaceReportKey reportKey)
        {
            _logger.LogDebug($"New workspace calculation ({reportKey}) received to the channel.");

            // todo: we need a fallback process if this fails.
            if (!_channel.Writer.TryWrite(reportKey) && _logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Could not add workspace calculation ({reportKey}) to the channel.");
            }
        }

        public IAsyncEnumerable<WorkspaceReportKey> GetNextCalculationAsync(CancellationToken ct)
        {
            return _channel.Reader.ReadAllAsync(ct);
        }
    }
}
