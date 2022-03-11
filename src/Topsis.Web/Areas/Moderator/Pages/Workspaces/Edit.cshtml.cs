using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application;
using Topsis.Application.Features;
using Topsis.Domain;
using Topsis.Domain.Specifications;

namespace Topsis.Web.Areas.Moderator.Pages.Workspaces
{
    public class EditModel : PageModel
    {
        private readonly IMessageBus _bus;

        [BindProperty]
        public Workspace Data { get; set; }

        public IDictionary<WorkspaceStatus, bool> StatusOptions { get; private set; }
        public QuestionnaireSettings Settings { get; private set; }

        public EditModel(IMessageBus bus)
        {
            _bus = bus;
        }

        public async Task OnGetAsync(string id)
        {
            await LoadPageAsync(id);
        }

        private async Task LoadPageAsync(string id)
        {
            var response = await _bus.SendAsync(new GetWorkspace.ById.Request(id));
            Data = response.Result;
            StatusOptions = GetStatusOptions(response.Result);
            Settings = Data.Questionnaire.GetSettings();
        }

        private IDictionary<WorkspaceStatus, bool> GetStatusOptions(Workspace workspace)
        {
            var result = new Dictionary<WorkspaceStatus, bool>();

            var spec = new WorkspaceStatusChangeSpec(workspace.CurrentStatus);
            foreach (var item in Enum.GetValues(typeof(WorkspaceStatus)))
            {
                var status = (WorkspaceStatus)item;
                if (spec.IsSatisfiedBy(status))
                {
                    result[status] = status == workspace.CurrentStatus;
                }
            }

            return result;
        }

        public IEnumerable<SelectListItem> GetScaleOptions()
        {
            foreach (var item in Enum.GetValues(typeof(OutputLinguisticScale)))
            {
                var scale = (OutputLinguisticScale)item;
                if (scale == OutputLinguisticScale.Unknown)
                {
                    continue;
                }

                yield return new SelectListItem(scale.ToString(), ((int)scale).ToString(), Settings.Scale == scale);
            }
        }

        public string GetScaleLabels(QuestionnaireSettings settings)
        {
            return string.Join(",", settings.AlternativeRange.Select(x => x.Name));
        }

        public string GetWeightLabels(QuestionnaireSettings settings)
        {
            return string.Join(",", settings.CriteriaWeightRange.Select(x => x.Name));
        }

        public async Task OnPostChangeOrder(string id, int? criterionId, int? alternativeId, bool moveUp)
        {
            var cmd = new EditWorkspace.ChangeOrderCommand() 
            { 
                WorkspaceId = id, 
                CriterionId = criterionId,
                AlternativeId = alternativeId, 
                MoveUp = moveUp 
            };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task OnPostChangeInfo(string id, string title, string description)
        {
            var cmd = new EditWorkspace.ChangeInfoCommand()
            {
                WorkspaceId = id,
                Title = title,
                Description = description
            };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task OnPostChangeStatus(string id, WorkspaceStatus status)
        {
            var cmd = new EditWorkspace.ChangeStatusCommand()
            {
                WorkspaceId = id,
                Status = status
            };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task OnPostAddCriterion(string id, string title)
        {
            var cmd = new EditWorkspace.AddCriterionCommand()
            {
                WorkspaceId = id,
                Title = title ?? "new criterion"
            };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task OnPostAddAlternative(string id, string title)
        {
            var cmd = new EditWorkspace.AddAlternativeCommand()
            {
                WorkspaceId = id,
                Title = title ?? "new alternative"
            };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task OnPostDeleteCriterion(string id, int criterionId)
        {
            var cmd = new EditWorkspace.DeleteCriterionCommand()
            {
                WorkspaceId = id,
                CriterionId = criterionId
            };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task OnPostDeleteAlternative(string id, int alternativeId)
        {
            var cmd = new EditWorkspace.DeleteAlternativeCommand()
            {
                WorkspaceId = id,
                AlternativeId = alternativeId
            };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task OnPostChangeCriterion(EditWorkspace.ChangeCriterionCommand cmd)
        {
            await _bus.SendAsync(cmd);
            await LoadPageAsync(cmd.Id);
        }

        public async Task OnPostChangeAlternative(string id, int alternativeId, string title)
        {
            var cmd = new EditWorkspace.ChangeAlternativeCommand()
            {
                WorkspaceId = id,
                AlternativeId = alternativeId,
                Title = title
            };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task OnPostChangeAlternativesRange(EditWorkspace.ChangeAlternativesRangeCommand cmd)
        {
            await _bus.SendAsync(cmd);
            await LoadPageAsync(cmd.WorkspaceId);
        }

        public async Task OnPostChangeCriteriaWeightsRange(EditWorkspace.ChangeCriteriaWeightRangeCommand cmd)
        {
            await _bus.SendAsync(cmd);
            await LoadPageAsync(cmd.WorkspaceId);
        }

        public async Task OnPostChangeTopsisSettings(EditWorkspace.ChangeTopsisSettingsCommand cmd)
        {
            await _bus.SendAsync(cmd);
            await LoadPageAsync(cmd.WorkspaceId);
        }

        public async Task OnPostAddCriterionOption(string id)
        {
            var cmd = new EditWorkspace.AddCriterionOptionCommand { WorkspaceId = id };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task OnPostDeleteCriterionOption(string id, int index)
        {
            var cmd = new EditWorkspace.DeleteCriterionOptionCommand { WorkspaceId = id, Index = index };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task OnPostCalculateResultsAsync(string id, AlgorithmType algorithm)
        {
            var cmd = new CalculateResults.Command { WorkspaceId = id, Algorithm = algorithm };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task OnPostClearVotesAsync(string id)
        {
            var cmd = new EditWorkspace.ClearVotesCommand { WorkspaceId = id };

            await _bus.SendAsync(cmd);
            await LoadPageAsync(id);
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var cmd = new EditWorkspace.DeleteCommand { WorkspaceId = id };

            await _bus.SendAsync(cmd);
            return RedirectToPage("Index");
        }
    }
}
