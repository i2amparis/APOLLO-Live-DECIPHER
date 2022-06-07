using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Results;
using Topsis.Domain;
using Topsis.Domain.Common;

namespace Topsis.Application.Features
{
    public static class CalculateResults
    {
        public class Command : IRequest<string>
        {
            public string WorkspaceId { get; set; }
            public AlgorithmType Algorithm { get; set; }
        }

        public class PrecalculateCommand : IRequest<PrecalculationResult>
        {
            public string WorkspaceId { get; set; }
        }

        public class Handler :
            IRequestHandler<PrecalculateCommand, PrecalculationResult>,
            IRequestHandler<Command, string>
        {
            private readonly IWorkspaceRepository _workspaces;
            private readonly ICalculateResultsProducer _calculationProducer;
            private readonly ICalculateResultsProcessor _calculationProcessor;

            public Handler(IWorkspaceRepository workspaces,
                IWorkspaceReportRepository workspaceReportRepository,
                ICalculateResultsProducer calculationProducer,
                ICalculateResultsProcessor calculationProcessor)
            {
                _workspaces = workspaces;
                _calculationProducer = calculationProducer;
                _calculationProcessor = calculationProcessor;
            }

            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var workspaceId = command.WorkspaceId.DehashInts().First();
                var item = await _workspaces.GetByIdForCalculationAsync(workspaceId);
                if (item.IsFinalized() == false)
                {
                    throw new DomainException(DomainErrors.WorkspaceStatus_CannotCalculateResults, $"status: {item.CurrentStatus}");
                }

                var report = item.CreateOrUpdateReport(command.Algorithm);
                await _workspaces.UnitOfWork.SaveChangesAsync();

                // Send to queue.
                _calculationProducer.AddCalculationInQueue(new WorkspaceReportKey(workspaceId, report.Id));
                return item.Id.Hash();
            }

            public async Task<PrecalculationResult> Handle(PrecalculateCommand command, CancellationToken cancellationToken)
            {
                var workspaceId = command.WorkspaceId.DehashInts().First();
                return await _calculationProcessor.PrecalculateAsync(workspaceId);
            }
        }
    }
}
