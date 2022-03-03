using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Topsis.Application.Contracts.Database;
using Topsis.Domain;

namespace Topsis.Web.ChartJs
{
    public class ChartJsReport
    {
        private static readonly string[] Colors = new []
        {
            "rgba(47, 75, 124, 0.2)", // "#2f4b7c",
            "rgba(255, 166, 0, 0.2)", // "#ffa600",
            "rgba(255, 124, 67, 0.2)", // "#ff7c43",
            "rgba(249, 93, 106, 0.2)", // "#f95d6a",
            "rgba(212, 80, 135, 0.2)", // "#d45087",
            "rgba(160, 81, 149, 0.2)", // "#a05195",
            "rgba(102, 81, 145, 0.2)", // "#665191",
            "rgba(0, 63, 92, 0.2)", // #665191
        };

        //private const string Color1 = "rgba(255, 99, 132, 0.2)";
        //private const string Color2 = "rgba(54, 162, 235, 0.2)";

        private static string Color1 = Colors[1];
        private static string Color2 = Colors[4];

        public ChartJsReport()
        {
            Type = "bar";
        }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public ChartJsData Data { get; set; }

        [JsonProperty("options")]
        public ChartJsDatasetOptions Options { get; set; }

        public static ChartJsReport BuildAlternativesReport(WorkspaceReportViewModel vm)
        {
            var labels = new List<string>();
            var myData = new List<double>();
            var groupData = new List<double>();

            foreach (var item in vm.ChartAlternatives.OrderBy(x => x.AlternativeOrder))
            {
                labels.Add(item.AlternativeTitle);
                myData.Add(item.StakeholderTopsis);
                groupData.Add(item.GroupTopsis);
            }

            var settings = vm.Workspace.Questionnaire.GetSettings();
            var suggestedMaxY = (int)Math.Ceiling(Math.Max(myData.Max(), groupData.Max()));
            var suggestedMinY = Math.Round(Math.Max(0, groupData.Min() - 0.2), 1);

            var scales = new ChartJsDatasetOptions.ChartJsScales(
                new ChartJsDatasetOptions.ChartJsAxes("Alternatives"),
                new ChartJsDatasetOptions.ChartJsAxes("Evaluation", suggestedMax: suggestedMaxY, ticksMin: suggestedMinY, stepSize:1));

            return new ChartJsReport()
            {
                Data = new ChartJsData
                {
                    Labels = labels,
                    Datasets = new[] {
                            new ChartJsDataset
                            {
                                BackgroundColor = Color1,
                                Label = "My Vote",
                                Data = myData
                            },
                            new ChartJsDataset
                            {
                                BackgroundColor = Color2,
                                Label = "Group Vote",
                                Data = groupData
                            }
                        }
                },
                Type = "bar",
                Options = new ChartJsDatasetOptions
                {
                    Scales = scales
                }
            };
        }

        internal static ChartJsReport BuildConsensusDegreeReport(WorkspaceReportViewModel vm)
        {
            if (vm.AlternativesConsensusDegree == null)
            {
                return null;
            }

            if (vm.ChartGroups.TryGetValue(StakeholderTopsis.DefaultGroupName, out var globalTopsis) == false)
            {
                return null;
            }

            var data = new List<ChartJsXY>();
            var labels = new List<string>();
            var globalTopsisDict = globalTopsis.ToDictionary(x => x.AlternativeId, x => x.Topsis);

            foreach (var item in vm.Workspace.Questionnaire.Alternatives.OrderBy(x => x.Order))
            {
                if (vm.AlternativesConsensusDegree.TryGetValue(item.Id, out var consensusDegree) == false
                    || globalTopsisDict.TryGetValue(item.Id, out var topsis) == false)
                {
                    continue;
                }

                data.Add(new ChartJsXY(topsis, consensusDegree));
                labels.Add(item.Title);
            }

            return new ChartJsReport()
            {
                Data = new ChartJsData
                {
                    Labels = labels,
                    Datasets = new object[] {
                        new ChartJsDatasetXY
                        {
                            BackgroundColor = Color1,
                            Label = "",
                            Data = data
                        }
                    }
                },
                Type = "scatter",
                Options = new ChartJsDatasetOptions
                {
                    Scales = new ChartJsDatasetOptions.ChartJsScales("Group Topsis", "Degree")
                }
            };
        }

        internal static ChartJsReport BuildCategoriesReport(WorkspaceReportViewModel vm)
        {
            if (vm.ChartGroups == null)
            {
                return null;
            }

            var alternativeDict = vm.Workspace.Questionnaire.AlternativesDictionary;

            var datasets = vm.ChartGroups.Select(x => new ChartJsDataset
            {
                Label = x.Key,
                BackgroundColor = GetRandomColor(),
                Data = x.Value.OrderBy(x => alternativeDict[x.AlternativeId].Order).Select(x => x.Topsis).ToList()
            }).ToList();

            for (int i = 0; i < datasets.Count; i++)
            {
                var index = i % Colors.Length;
                datasets[i].BackgroundColor = Colors[index];
            }

            var labels = alternativeDict.Values.OrderBy(x => x.Order).Select(x => x.Title).ToList();
            var settings = vm.Workspace.Questionnaire.GetSettings();

            var suggestedMaxY = (int)settings.Scale;
            var scales = new ChartJsDatasetOptions.ChartJsScales(
                new ChartJsDatasetOptions.ChartJsAxes("Alternatives"),
                new ChartJsDatasetOptions.ChartJsAxes("Evaluation", suggestedMax: suggestedMaxY, stepSize: 1));

            return new ChartJsReport()
            {
                Data = new ChartJsData
                {
                    Labels = labels,
                    Datasets = datasets.ToArray()
                },
                Type = "bar",
                Options = new ChartJsDatasetOptions
                { 
                    Scales = scales
                }
            };
        }

        internal static ChartJsReport BuildConsensusCompareReport(WorkspaceReportViewModel vm)
        {
            if (vm.ChartConsensus?.Count > 0 != true || vm.ChartConsensus.ContainsKey(vm.UserId) == false)
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
                                BackgroundColor = Color1,
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
                                BackgroundColor = Color2,
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
            if (vm.ChartConsensus?.Count > 0 != true)
            {
                return null;
            }

            var data = vm.ChartConsensus.Where(x => x.Key != vm.UserId).Select(x => x.Value).ToList();
            var colors = Enumerable.Range(0, data.Count).Select(x => Color2).ToList();
            var labels = Enumerable.Range(1, data.Count).Select(x => $"S{x}").ToList();

            if (vm.ChartConsensus.TryGetValue(vm.UserId, out var currentStakeHolderConsensus))
            {
                // add current stakeholder bar in different color.
                data.Insert(0, currentStakeHolderConsensus);
                colors.Insert(0, Color1);
                labels.Insert(0, "Me");
            }

            var avgConsensus = data.Average();
            var avgData = Enumerable.Range(1, data.Count).Select(x => avgConsensus).ToList();

            var yMax = Math.Ceiling(avgData.Max());
            var yMin = Math.Round(Math.Max(0, avgData.Min() - 0.2), 1);
            var scales = new ChartJsDatasetOptions.ChartJsScales(
                new ChartJsDatasetOptions.ChartJsAxes("Stakeholders"),
                new ChartJsDatasetOptions.ChartJsAxes("Consensus", suggestedMin: yMin, suggestedMax: yMax, stepSize:0.2, ticksBeginAtZero: false));
            return new ChartJsReport()
            {
                Data = new ChartJsData
                {
                    Labels = labels,
                    Datasets = new[] {
                            new ChartJsDataset
                            {
                                Type = "bar",
                                BackgroundColor = colors,
                                Label = "Consensus",
                                Data = data,
                                Fill = true,
                                PointBackgroundColor = "rgb(255, 99, 132)",
                                PointBorderColor= "#fff",
                                PointHoverBackgroundColor= "#fff",
                                PointHoverBorderColor= "rgb(255, 99, 132)"
                            },
                            new ChartJsDataset
                            {
                                Type = "line",
                                BackgroundColor = Color1,
                                Label = "Average Consensus",
                                Data = avgData,
                            }
                        }
                },
                Options = new ChartJsDatasetOptions
                {
                    Scales = scales
                }
            };
        }

        private static string GetRandomColor()
        {
            var red = new Random().Next(5, 250);
            var green = new Random().Next(5, 250);
            var blue = new Random().Next(5, 250);
            return $"rgba({red}, {green}, {blue}, 0.2)";
        }
    }
}
