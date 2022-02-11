using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;
using Topsis.Domain.Common;

namespace Topsis.Web.Areas.Moderator.Pages.Workspaces
{
    public class DataModel : PageModel
    {
        public StakeholderAnswerDto[] Answers { get; private set; }
        public WorkspaceReport Report { get; private set; }
        public Workspace Data { get; private set; }
        public WorkspaceAnalysisResult Analysis { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, 
            [FromServices] IReportService reports,
            [FromServices] IWorkspaceRepository workspaces)
        {
            var workspaceId = id.DehashInts().First();
            Data = await workspaces.GetByIdForCalculationAsync(workspaceId);
            Analysis = Data.GetReportData();
            if (Analysis == null)
            {
                return RedirectToPage("Edit", new { id });
            }

            Answers = await reports.GetAnswersForCalculationAsync(workspaceId);

            return Page();
        }
    }
}
