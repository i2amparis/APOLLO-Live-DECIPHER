using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Results;
using Topsis.Domain;
using Topsis.Domain.Common;

namespace Topsis.Application.Features
{

    public static class EditWorkspace
    {
        #region [ Commands ]
        public class AddAlternativeCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
            public string Title { get; set; }
        }

        public class AddCriterionCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
            public string Title { get; set; }
        }

        public class DeleteAlternativeCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
            public int AlternativeId { get; set; }
        }

        public class DeleteCriterionCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
            public int CriterionId { get; set; }
        }

        public class ChangeStatusCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
            public WorkspaceStatus Status { get; set; }
        }

        public class ChangeInfoCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
        }

        public class ChangeInfoCommandValidator : AbstractValidator<ChangeInfoCommand>
        {
            public ChangeInfoCommandValidator()
            {
                RuleFor(m => m.WorkspaceId).NotNull();
                RuleFor(m => m.Title).NotNull().Length(3, 255);
                RuleFor(m => m.Description).NotNull().Length(3, 512);
            }
        }

        public class ChangeOrderCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
            public int? CriterionId { get; set; }
            public int? AlternativeId { get; set; }
            public bool MoveUp { get; set; }
        }

        public class ChangeCriterionCommand : IRequest<string>
        {
            public string Id { get; set; }
            public int CriterionId { get; set; }
            public CriterionType CriterionType { get; set; }
            public string Title { get; set; }
        }

        public class ChangeAlternativeCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
            public int AlternativeId { get; set; }
            public string Title { get; set; }
        }

        public class ChangeWeightCommand : IRequest<string>
        { 
            public string WorkspaceId { get; set; }
            public double Weight { get; set; }
        }

        public class ChangeCriterionTypeCommand : IRequest<string>
        { 
            public string WorkspaceId { get; set; }
            public int CriterionId { get; set; }
            public CriterionType Type { get; set; }
        }

        public class AddCriterionOptionCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
        }

        public class DeleteCriterionOptionCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
            public int Index { get; set; }
        }

        public class ChangeTopsisSettingsCommand : IRequest<string>
        { 
            public double Rigorousness { get; set; }
            public string WorkspaceId { get; set; }
            public OutputLinguisticScale Scale { get; set; }
            public int CriterionWeightMax { get; set; }
        }

        public class ChangeAlternativesRangeCommand : IRequest<string>
        {
            public List<NameValueOption> Range { get; set; }
            public string WorkspaceId { get; set; }
        }

        public class ChangeCriteriaWeightRangeCommand : IRequest<string>
        {
            public List<NameValueOption> Range { get; set; }
            public string WorkspaceId { get; set; }
        }

        public class ClearVotesCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
        }

        public class DeleteCommand : IRequest<string>
        {
            public string WorkspaceId { get; set; }
        }
        #endregion

        public class Handler : 
            IRequestHandler<ChangeOrderCommand, string>,
            IRequestHandler<ChangeInfoCommand, string>,
            IRequestHandler<ChangeStatusCommand, string>,
            IRequestHandler<AddCriterionCommand, string>,
            IRequestHandler<AddAlternativeCommand, string>,
            IRequestHandler<DeleteCriterionCommand, string>,
            IRequestHandler<DeleteAlternativeCommand, string>,
            IRequestHandler<ChangeCriterionCommand, string>,
            IRequestHandler<ChangeAlternativeCommand, string>,
            IRequestHandler<ChangeTopsisSettingsCommand, string>,
            IRequestHandler<ChangeAlternativesRangeCommand, string>,
            IRequestHandler<ChangeCriteriaWeightRangeCommand, string>,
            IRequestHandler<AddCriterionOptionCommand, string>,
            IRequestHandler<DeleteCriterionOptionCommand, string>,
            IRequestHandler<ClearVotesCommand, string>,
            IRequestHandler<DeleteCommand, string>
        {
            private readonly IWorkspaceRepository _workspaces;
            private readonly IReportService _reports;
            private readonly IWorkspaceNotificationService _notifications;
            private readonly ILogger<Handler> _log;

            public Handler(IWorkspaceRepository workspaces, 
                IReportService reports, 
                IWorkspaceNotificationService notifications,
                ILogger<Handler> log)
            {
                _workspaces = workspaces;
                _reports = reports;
                _notifications = notifications;
                _log = log;
            }

            public async Task<string> Handle(ChangeOrderCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);

                if (command.CriterionId.HasValue)
                {
                    item.MoveCriterion(command.CriterionId.Value, command.MoveUp);
                }

                if (command.AlternativeId.HasValue)
                {
                    item.MoveAlternative(command.AlternativeId.Value, command.MoveUp);
                }

                return await SaveAsync(item);
            }

            public async Task<string> Handle(ChangeInfoCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.ChangeInfo(command.Title, command.Description);

                return await SaveAsync(item);
            }

            public async Task<string> Handle(ChangeStatusCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.ChangeStatus(command.Status);
                var result = await SaveAsync(item);
                
                await _notifications.OnWorkspaceStatusChangedAsync(new WorkspaceStatusChangedMessage(item.Id, item.CurrentStatus));

                return result;
            }

            public async Task<string> Handle(AddCriterionCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.AddCriterion(command.Title);
                return await SaveAsync(item);
            }

            public async Task<string> Handle(AddAlternativeCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.AddAlternative(command.Title);
                return await SaveAsync(item);
            }

            public async Task<string> Handle(DeleteCriterionCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.RemoveCriterion(command.CriterionId);
                return await SaveAsync(item);
            }

            public async Task<string> Handle(DeleteAlternativeCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.RemoveAlternative(command.AlternativeId);
                return await SaveAsync(item);
            }

            public async Task<string> Handle(ChangeCriterionCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.Id);
                item.ChangeCriterion(command.CriterionId, command.Title, command.CriterionType);
                return await SaveAsync(item);
            }

            public async Task<string> Handle(ChangeAlternativeCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.ChangeAlternative(command.AlternativeId, command.Title);
                return await SaveAsync(item);
            }

            public async Task<string> Handle(ChangeTopsisSettingsCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.ChangeQuestionnaireSettings(command.Scale, command.Rigorousness, command.CriterionWeightMax);
                return await SaveAsync(item);
            }

            public async Task<string> Handle(AddCriterionOptionCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.AddCriterionOption();
                return await SaveAsync(item);
            }

            public async Task<string> Handle(DeleteCriterionOptionCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.DeleteCriterionOption(command.Index);
                return await SaveAsync(item);
            }

            public async Task<string> Handle(ChangeAlternativesRangeCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.ChangeAlternativeRange(command.Range);
                return await SaveAsync(item);
            }

            public async Task<string> Handle(ChangeCriteriaWeightRangeCommand command, CancellationToken cancellationToken)
            {
                var item = await LoadWorkspaceAsync(command.WorkspaceId);
                item.ChangeCriteriaWeightsRange(command.Range);
                return await SaveAsync(item);
            }

            public async Task<string> Handle(ClearVotesCommand command, CancellationToken cancellationToken)
            {
                var id = command.WorkspaceId.DehashInts().First();
                await _workspaces.ClearVotesAndReportsAsync(id);
                await _workspaces.UnitOfWork.SaveChangesAsync();
                return id.Hash();
            }

            public async Task<string> Handle(DeleteCommand command, CancellationToken cancellationToken)
            {
                var id = command.WorkspaceId.DehashInts().First();
                var workspace = await _workspaces.GetByIdAsync(id);
                if (workspace == null)
                {
                    return null;
                }

                var sw = Stopwatch.StartNew();
                await _workspaces.DeleteAsync(workspace);
                await _workspaces.UnitOfWork.SaveChangesAsync();
                sw.Stop();
                _log.LogInformation($"Deletion took {sw.ElapsedMilliseconds / 1000} secs.");
                return id.Hash();
            }

            #region [ Helpers ]
            private async Task<Workspace> LoadWorkspaceAsync(string hashId)
            {
                var id = hashId.DehashInts().First();
                var item = await _workspaces.GetByIdAsync(id);
                return item;
            }

            private async Task<string> SaveAsync(Workspace item)
            {
                await _workspaces.UnitOfWork.SaveChangesAsync();
                _reports.ClearWorkspaceCache(item.Id);
                return item.Id.Hash();
            }
            #endregion
        }
    }
}
