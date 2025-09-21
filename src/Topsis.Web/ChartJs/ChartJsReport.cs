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
            // https://colorbrewer2.org/#type=diverging&scheme=RdYlBu&n=10
         /*
118,42,131
153,112,171
194,165,207
231,212,232
217,240,211
166,219,160
90,174,97
27,120,55
          */
            "rgba(118,42,131, 0.8)", // "#d7191c",
            "rgba(153,112,171, 0.8)", // "#fdae61",
            "rgba(194,165,207, 0.8)", // "#ffffbf",
            "rgba(231,212,232, 0.8)", // "#abd9e9",
            "rgba(217,240,211, 0.8)", // "#2c7bb6",
            "rgba(166,219,160, 0.8)", // "#a50026",
            "rgba(90,174,97, 0.8)", // "#a6cee3"
            "rgba(27,120,55, 0.8)", // #1f78b4
        };

        private static string MyVoteColor = "rgba(166, 206, 227, 0.8)"; // "#a6cee3"
        private static string GroupVoteColor = "rgba(31, 120, 180, 0.8)"; // #1f78b4

        private static string PreviousRoundColor = "rgba(166, 206, 227, 0.8)";
        private static string CurrentRoundColor = "rgba(31, 120, 180, 0.8)";

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
            var suggestedMaxY = myData.Count > 0 ? (int)Math.Ceiling(Math.Max(myData.Max(), groupData.Max())) : 0;
            var suggestedMinY = myData.Count > 0 ? (int)Math.Floor(Math.Round(Math.Max(0, groupData.Min() - 0.2), 1)) : 0;

            var scales = new ChartJsDatasetOptions.ChartJsScales(
                new ChartJsDatasetOptions.ChartJsAxes("Indicators"),
                new ChartJsDatasetOptions.ChartJsAxes("Evaluation", suggestedMin: suggestedMinY, suggestedMax: suggestedMaxY, ticksMin: suggestedMinY, stepSize:1));

            return new ChartJsReport()
            {
                Data = new ChartJsData
                {
                    Labels = labels,
                    Datasets = new[] {
                            new ChartJsDataset
                            {
                                BackgroundColor = MyVoteColor,
                                Label = "My selection",
                                Data = myData
                            },
                            new ChartJsDataset
                            {
                                BackgroundColor = GroupVoteColor,
                                Label = "All respondents",
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
                            BackgroundColor = MyVoteColor,
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
                Label = x.Key == StakeholderTopsis.DefaultGroupName ? "All respondents" : x.Key,
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

            var suggestedMaxY = (int)Math.Ceiling(datasets.Max(x => x.Data.Max()));
            var suggestedMinY = Math.Max(0, (int)Math.Floor(datasets.Max(x => x.Data.Max()) - 0.2));

            var scales = new ChartJsDatasetOptions.ChartJsScales(
                new ChartJsDatasetOptions.ChartJsAxes("Indicators"),
                new ChartJsDatasetOptions.ChartJsAxes("Evaluation", suggestedMin: suggestedMinY, suggestedMax: suggestedMaxY, stepSize: 1));

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
                                BackgroundColor = MyVoteColor,
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
                                BackgroundColor = GroupVoteColor,
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
            var colors = Enumerable.Range(0, data.Count).Select(x => GroupVoteColor).ToList();
            var labels = Enumerable.Range(1, data.Count).Select(x => $"S{x}").ToList();

            if (vm.ChartConsensus.TryGetValue(vm.UserId, out var currentStakeHolderConsensus))
            {
                // add current stakeholder bar in different color.
                data.Insert(0, currentStakeHolderConsensus);
                colors.Insert(0, MyVoteColor);
                labels.Insert(0, "Me");
            }

            var avgConsensus = data.Average();
            var avgData = Enumerable.Range(1, data.Count).Select(x => avgConsensus).ToList();

            var yMax = (int)Math.Ceiling(avgData.Max());
            var yMin = (int)Math.Floor(Math.Round(Math.Max(0, avgData.Min() - 0.2), 1));
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
                                BackgroundColor = MyVoteColor,
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

        internal static ChartJsReport BuildReportComparison(WorkspaceReportViewModel vm)
        {
            if (vm.ReportComparison == null)
            {
                return null;
            }

            var labels = vm.ChartAlternatives.OrderBy(x => x.AlternativeOrder).Select(x => x.AlternativeTitle).ToList();

            int count = 1;
            var datasets = new List<ChartJsDataset>();

            var currentRound = vm.ReportComparison.Keys.Max();
            foreach (var item in vm.ReportComparison.OrderBy(x => x.Key))
            {
                var color = item.Key == currentRound ? CurrentRoundColor : PreviousRoundColor;
                datasets.Add(new ChartJsDataset
                {
                     BackgroundColor = color,
                     Label = $"{item.Key} Round",
                     Data = item.Value.OrderBy(x => x.AlternativeOrder).Select(x => x.GroupTopsis).ToList()
                });
                count++;
            }

            var settings = vm.Workspace.Questionnaire.GetSettings();
            var suggestedMaxY = (int)Math.Ceiling(datasets.Select(x => x.Data.Max()).Max());
            var suggestedMinY = (int)Math.Floor(datasets.Select(x => x.Data.Min()).Min());

            var scales = new ChartJsDatasetOptions.ChartJsScales(
                new ChartJsDatasetOptions.ChartJsAxes("Indicators"),
                new ChartJsDatasetOptions.ChartJsAxes("Evaluation", suggestedMin: suggestedMinY, suggestedMax: suggestedMaxY, ticksMin: suggestedMinY, stepSize: 1));

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

        private static string GetRandomColor()
        {
            var red = new Random().Next(5, 250);
            var green = new Random().Next(5, 250);
            var blue = new Random().Next(5, 250);
            return $"rgba({red}, {green}, {blue}, 0.2)";
        }
    }
}
