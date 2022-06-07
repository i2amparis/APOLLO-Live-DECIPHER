using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Results;
using Topsis.Domain;

namespace Topsis.Adapters.Algorithm.Queue
{
    

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
                var result = await GetAnalysisResultAsync(reportKey.WorkspaceId, algorithm, reports, workspace);

                // save
                logger.LogDebug($"Finalizing workspace result: {reportKey}");
                workspace.FinalizeReport(reportKey.WorkspaceReportId, result);
                await repository.UnitOfWork.SaveChangesAsync();

                reports.ClearWorkspaceCache(workspace.Id);
                logger.LogDebug($"Workspace report finalized: {reportKey}");
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, ex.Message, reportKey);
                //throw;
            }
        }

        private static async Task<WorkspaceAnalysisResult> GetAnalysisResultAsync(int workspaceId, ITopsisAlgorithm algorithm, IReportService reports, Workspace workspace)
        {
            var answers = await reports.GetAnswersForCalculationAsync(workspaceId);
            var stakeholdersDemographics = reports.GetStakeholdersDemographicsAsync(answers.Select(x => x.StakeholderId).Distinct().ToArray());
            var jobCategories = await reports.GetJobCategoriesAsync();
            return await algorithm.AnalyzeAsync(workspace, jobCategories, answers, stakeholdersDemographics);
        }

        public async Task<WorkspaceAnalysisResult> PrecalculateAsync(int workspaceId)
        {
            using var scope = _scopeFactory.CreateScope();
            var reports = scope.ServiceProvider.GetRequiredService<IReportService>();
            var repository = scope.ServiceProvider.GetRequiredService<IWorkspaceRepository>();
            var workspace = await repository.GetByIdForCalculationAsync(workspaceId);
            return await GetAnalysisResultAsync(workspaceId, _algorithm, reports, workspace);
        }
    }
}
