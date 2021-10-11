using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Adapters.Algorithm.Queue;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Results;

namespace Topsis.AcceptanceTests.Fakes
{
    /// <summary>
    /// Fake Producer acts as a consumer too for testing purposes.
    /// </summary>
    public class InProcessCalculateResultsProducer : ICalculateResultsProducer
    {
        private readonly ICalculateResultsProcessor _processor;

        public InProcessCalculateResultsProducer(ICalculateResultsProcessor processor)
        {
            _processor = processor;
        }

        public void AddCalculationInQueue(WorkspaceReportKey reportKey)
        {
            _processor.ProcessAsync(reportKey).GetAwaiter().GetResult();
        }

        public async IAsyncEnumerable<WorkspaceReportKey> GetNextCalculationAsync(CancellationToken ct)
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}
