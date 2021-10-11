using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Database;

namespace Topsis.Adapters.Algorithm.Queue
{
    public interface ICalculateResultsProcessor
    {
        Task ProcessAsync(WorkspaceReportKey reportKey);
    }

    public class Processor : ICalculateResultsProcessor
    {
        private readonly ITopsisAlgorithm _algorithm;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<Processor> _logger;

        public Processor(ITopsisAlgorithm algorithm,
            IServiceScopeFactory scopeFactory,
            ILogger<Processor> logger)
        {
            _algorithm = algorithm;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task ProcessAsync(WorkspaceReportKey reportKey)
        {
            _logger.LogDebug($"Consuming workspace report ({reportKey})");
            using var scope = _scopeFactory.CreateScope();
            var reports = scope.ServiceProvider.GetRequiredService<IReportService>();
            var repository = scope.ServiceProvider.GetRequiredService<IWorkspaceRepository>();

            _logger.LogDebug($"Fetched workspace report data: {reportKey}");

            await DoWorkAsync(reportKey, _algorithm, repository, reports, _logger);
        }

        public static async Task DoWorkAsync(WorkspaceReportKey reportKey,
            ITopsisAlgorithm algorithm,
            IWorkspaceRepository repository,
            IReportService reports,
            ILogger logger)
        {
            try
            {
                var workspace = await repository.GetByIdForCalculationAsync(reportKey.WorkspaceId);
                var answers = await reports.GetAnswersForCalculationAsync(reportKey.WorkspaceId);
                var result = await algorithm.AnalyzeAsync(workspace, answers);

                // save
                logger.LogDebug($"Finalizing workspace result: {reportKey}");
                workspace.FinalizeReport(reportKey.WorkspaceReportId, result);
                await repository.UnitOfWork.SaveChangesAsync();

                logger.LogDebug($"Workspace report finalized: {reportKey}");
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, ex.Message, reportKey);
                throw;
            }
        }
    }
}
