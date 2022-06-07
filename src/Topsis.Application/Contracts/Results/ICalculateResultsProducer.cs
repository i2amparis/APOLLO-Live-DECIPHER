using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;

namespace Topsis.Application.Contracts.Results
{
    public interface ICalculateResultsProducer
    {
        void AddCalculationInQueue(WorkspaceReportKey reportKey);
        IAsyncEnumerable<WorkspaceReportKey> GetNextCalculationAsync(CancellationToken ct);
    }

    public interface ICalculateResultsProcessor
    {
        Task ProcessAsync(WorkspaceReportKey reportKey);
        Task<PrecalculationResult> PrecalculateAsync(int workspaceId);
    }
}
