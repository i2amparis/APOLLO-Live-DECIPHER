using System.Collections.Generic;
using System.Threading;
using Topsis.Application.Contracts.Algorithm;

namespace Topsis.Application.Contracts.Results
{
    public interface ICalculateResultsProducer
    {
        void AddCalculationInQueue(WorkspaceReportKey reportKey);
        IAsyncEnumerable<WorkspaceReportKey> GetNextCalculationAsync(CancellationToken ct);
    }
}
