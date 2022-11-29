using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Topsis.Adapters.Algorithm.Queue;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Results;

namespace Topsis.AcceptanceTests.Fakes
{
    public class InProcessCalculateProcessor : ICalculateResultsProcessor
    {

        private IWorkspaceRepository _repository;
        private IReportService _reports;
        private ILogger _logger;
        private ITopsisAlgorithm _algorithm;

        public InProcessCalculateProcessor(ITopsisAlgorithm algorithm,
            IWorkspaceRepository repository,
            IReportService reports,
            ILogger<InProcessCalculateProcessor> logger)
        {
            _algorithm = algorithm;
            _repository = repository;
            _reports = reports;
            _logger = logger;
        }

        public Task<PrecalculationResult> PrecalculateAsync(int workspaceId)
        {
            throw new System.NotImplementedException();
        }

        public async Task ProcessAsync(WorkspaceReportKey reportKey)
        {
            await Processor.DoWorkAsync(reportKey,
                _algorithm,
                _repository,
                _reports,
                _logger);
        }
    }
}
