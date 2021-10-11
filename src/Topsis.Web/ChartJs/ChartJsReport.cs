using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Topsis.Application.Contracts.Database;

namespace Topsis.Web.ChartJs
{
    public class ChartJsReport
    {
        public ChartJsReport()
        {
            Type = "bar";
        }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public ChartJsData Data { get; set; }

        public static ChartJsReport BuildAlternativesReport(WorkspaceReportViewModel vm)
        {
            var labels = new List<string>();
            var myData = new List<double>();
            var avgData = new List<double>();
            foreach (var item in vm.ChartAlternatives)
            {
                labels.Add(item.Alternative);
                myData.Add(item.StakeholderTopsis);
                avgData.Add(item.AverageTopsis);
            }

            return new ChartJsReport()
            {
                Data = new ChartJsData
                {
                    Labels = labels,
                    Datasets = new[] {
                            new ChartJsDataset
                            {
                                BackgroundColor = "rgba(255, 99, 132, 0.2)",
                                Label = "My Vote",
                                Data = myData
                            },
                            new ChartJsDataset
                            {
                                BackgroundColor = "rgba(54, 162, 235, 0.2)",
                                Label = "Avg Vote",
                                Data = avgData
                            }
                        }
                },
                Type = "bar"
            };
        }

        internal static ChartJsReport BuildConsensusReport(WorkspaceReportViewModel vm)
        {
            if (vm.ChartConsensus == null)
            {
                return null;
            }

            var stakeholdersData = vm.ChartConsensus.Values.ToList();
            var labels = Enumerable.Range(1, stakeholdersData.Count).Select(x => $"S{x}").ToList();

            return new ChartJsReport()
            {
                Data = new ChartJsData
                {
                    Labels = labels,
                    Datasets = new[] {
                            new ChartJsDataset
                            {
                                BackgroundColor = "rgba(255, 99, 132, 0.2)",
                                Label = "Consensus",
                                Data = stakeholdersData,
                                Fill = true,
                                PointBackgroundColor = "rgb(255, 99, 132)",
                                PointBorderColor= "#fff",
                                PointHoverBackgroundColor= "#fff",
                                PointHoverBorderColor= "rgb(255, 99, 132)"
                            }
                        }
                },
                Type = "radar"
            };
        }
    }
}
