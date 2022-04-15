using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Adapters.Algorithm;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;
using Topsis.Domain.Common;
using Topsis.Domain.Contracts;
using Topsis.Web.ChartJs;

namespace Topsis.Web.Areas.Guest.Pages.Workspace
{
    public class IndexModel : PageModel
    {
        public WorkspaceReportViewModel Data { get; set; }
        public string AlternativesReportJson { get; set; }
        public string AlternativesYLabelsJson { get; set; }
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
                var settings = Data.Workspace.Questionnaire.GetSettings();
                AlternativesReportJson = JsonConvert.SerializeObject(ChartJsReport.BuildAlternativesReport(Data));
                AlternativesYLabelsJson = JsonConvert.SerializeObject(settings.AlternativeRange.Select(x => x.Name).ToArray());
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
                if (vm.ChartConsensus == null)
                {
                    return;
                }

                MyConsensus = vm.MyConsensus;
                AverarageConsensus = vm.AverarageConsensus;
                Tips = vm.Tips;
            }

            public IList<FeedbackTip> Tips { get; }

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
