using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Topsis.Application.Contracts.Database;

namespace Topsis.Web.ChartJs
{
    public class ChartJsReport
    {
        private const string ColorPink = "rgba(255, 99, 132, 0.2)";
        private const string ColorBlue = "rgba(54, 162, 235, 0.2)";

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
            var groupData = new List<double>();
            foreach (var item in vm.ChartAlternatives)
            {
                labels.Add(item.Alternative);
                myData.Add(item.StakeholderTopsis);
                groupData.Add(item.GroupTopsis);
            }

            return new ChartJsReport()
            {
                Data = new ChartJsData
                {
                    Labels = labels,
                    Datasets = new[] {
                            new ChartJsDataset
                            {
                                BackgroundColor = ColorPink,
                                Label = "My Vote",
                                Data = myData
                            },
                            new ChartJsDataset
                            {
                                BackgroundColor = ColorBlue,
                                Label = "Group Vote",
                                Data = groupData
                            }
                        }
                },
                Type = "bar"
            };
        }

        internal static object BuildConsensusCompareReport(WorkspaceReportViewModel vm)
        {
            if (vm.ChartConsensus == null)
            {
                return null;
            }

            var stakeholdersData = new List<double> { vm.ChartConsensus[vm.UserId] };
            var avgData = new List<double> { vm.ChartConsensus.Values.Average() };
            
            return new ChartJsReport()
            {
                Data = new ChartJsData
                {
                    Labels = new List<string> { "Consensus" },
                    Datasets = new[] {
                            new ChartJsDataset
                            {
                                BackgroundColor = ColorPink,
                                Label = "My Consensus",
                                Data = stakeholdersData,
                                Fill = true,
                                PointBackgroundColor = "rgb(255, 99, 132)",
                                PointBorderColor= "#fff",
                                PointHoverBackgroundColor= "#fff",
                                PointHoverBorderColor= "rgb(255, 99, 132)"
                            },
                            new ChartJsDataset
                            {
                                BackgroundColor = ColorBlue,
                                Label = "Average Consensus",
                                Data = avgData,
                                Fill = true,
                                PointBackgroundColor = "rgb(255, 99, 132)",
                                PointBorderColor= "#fff",
                                PointHoverBackgroundColor= "#fff",
                                PointHoverBorderColor= "rgb(255, 99, 132)"
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
            var avgConsensus = stakeholdersData.Average();
            var avgData = Enumerable.Range(1, stakeholdersData.Count).Select(x => avgConsensus).ToList();
            return new ChartJsReport()
            {
                Data = new ChartJsData
                {
                    Labels = labels,
                    Datasets = new[] {
                            new ChartJsDataset
                            {
                                Type = "bar",
                                BackgroundColor = ColorPink,
                                Label = "Consensus",
                                Data = stakeholdersData,
                                Fill = true,
                                PointBackgroundColor = "rgb(255, 99, 132)",
                                PointBorderColor= "#fff",
                                PointHoverBackgroundColor= "#fff",
                                PointHoverBorderColor= "rgb(255, 99, 132)",
                                Options = new ChartJsDatasetOptions
                                { 
                                    Elements = new ChartJsDatasetOptions.ChartJsElement 
                                    { 
                                        Bar = new ChartJsDatasetOptions.ChartJsBarElement($"highlight()") 
                                    }
                                }
                            },
                            new ChartJsDataset
                            {
                                Type = "line",
                                BackgroundColor = ColorPink,
                                Label = "Average Consensus",
                                Data = avgData,
                            }
                        }
                }
            };
        }
    }
}
