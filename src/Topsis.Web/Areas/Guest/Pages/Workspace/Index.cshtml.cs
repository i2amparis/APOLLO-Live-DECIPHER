using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Domain.Common;
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
        public object ConsensusDegreeJson { get; private set; }
        public ConsensusCompareReport CompareReport { get; private set; }

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
                ConsensusDegreeJson = JsonConvert.SerializeObject(ChartJsReport.BuildConsensusDegreeReport(Data));

                CompareReport = new ConsensusCompareReport(Data);
            }

            return Page();
        }

        public class ConsensusCompareReport
        {
            private const string DefaultBadge = "badge-secondary";

            public ConsensusCompareReport(WorkspaceReportViewModel vm)
            {
                MyConsensus = Rounder.Round(100d * vm.ChartConsensus[vm.UserId], 1);
                AverarageConsensus = Rounder.Round(100d * vm.ChartConsensus.Values.Average(), 1);
            }

            public double MyConsensus { get; }
            public double AverarageConsensus { get; }

            public string AverageBadgeCls => DefaultBadge;
            public string MyBadgeCls => GetMyBadgeClass();

            private string GetMyBadgeClass()
            {
                var delta = MyConsensus - AverarageConsensus;
                var absDelta = Math.Abs(delta);
                if (absDelta < 0.5d)
                {
                    return DefaultBadge;
                }

                if (delta < 0)
                {
                    if (absDelta > 2)
                    {
                        return "badge-danger";
                    }

                    return "badge-warning";
                }

                return "badge-success";
            }

            //private bool FindDeviations()
            //{ 
            //    // foreach stakeholder
            //    //   foreach alternative
            //    //      dev-increase-decrease = abs(solution stakeholder - group solution)

            //    // find max = max(dev)
            //    // result -> ena suggestion alt 
            //    // report -> this is the alt with the biggest dev
            //    // re-answer -> mark alternative with the biggest dev
            //}
        }
    }
}
