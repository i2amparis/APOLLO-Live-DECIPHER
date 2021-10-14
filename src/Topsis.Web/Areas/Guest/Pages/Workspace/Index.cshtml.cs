using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Domain.Contracts;
using Topsis.Web.ChartJs;

namespace Topsis.Web.Areas.Guest.Pages.Workspace
{
    public class IndexModel : PageModel
    {
        public WorkspaceReportViewModel Data { get; set; }
        public string AlternativesReportJson { get; set; }
        public string ConsensusReportJson { get; set; }
        public string ConsensusCompareReportJson { get; set; }
        public string CategoriesReportJson { get; set; }

        public async Task<IActionResult> OnGetAsync(string id,
            [FromServices] IUserContext userContext,
            [FromServices] IReportService reports)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToPage("Index");
            }

            Data = await reports.GetWorkspaceReportAsync(id, userContext);
            if (Data?.Workspace == null)
            {
                return RedirectToPage("Index");
            }

            if (Data.Workspace.HasReport())
            {
                AlternativesReportJson = JsonConvert.SerializeObject(ChartJsReport.BuildAlternativesReport(Data));
                ConsensusReportJson = JsonConvert.SerializeObject(ChartJsReport.BuildConsensusReport(Data));
                ConsensusCompareReportJson = JsonConvert.SerializeObject(ChartJsReport.BuildConsensusCompareReport(Data));
                CategoriesReportJson = JsonConvert.SerializeObject(ChartJsReport.BuildCategoriesReport(Data));
            }

            return Page();
        }
    }
}
