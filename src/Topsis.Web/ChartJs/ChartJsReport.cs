﻿using Newtonsoft.Json;
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

        internal static ChartJsReport BuildCategoriesReport(WorkspaceReportViewModel vm)
        {
            var datasets = vm.ChartGroups.Select(x => new ChartJsDataset
            {
                Label = x.Key,
                BackgroundColor = GetRandomColor(),
                Data = x.Value.OrderBy(x => x.AlternativeId).Select(x => x.Topsis).ToList()
            }).ToArray();

            var labels = vm.Workspace.Questionnaire.Alternatives.OrderBy(x => x.Id).Select(x => x.Title).ToList();

            return new ChartJsReport()
            {
                Data = new ChartJsData
                {
                    Labels = labels,
                    Datasets = datasets
                },
                Type = "bar"
            };
        }

        private static string GetRandomColor()
        {
            var red = new Random().Next(5, 250);
            var green = new Random().Next(5, 250);
            var blue = new Random().Next(5, 250);
            return $"rgba({red}, {green}, {blue}, 0.2)";
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

            var data = vm.ChartConsensus.Where(x => x.Key != vm.UserId).Select(x => x.Value).ToList();
            var colors = Enumerable.Range(0, data.Count).Select(x => ColorBlue).ToList();
            var labels = Enumerable.Range(1, data.Count).Select(x => $"S{x}").ToList();

            if (vm.ChartConsensus.TryGetValue(vm.UserId, out var currentStakeHolderConsensus))
            {
                // add current stakeholder bar in different color.
                data.Insert(0, currentStakeHolderConsensus);
                colors.Insert(0, ColorPink);
                labels.Insert(0, "Me");
            }

            var avgConsensus = data.Average();
            var avgData = Enumerable.Range(1, data.Count).Select(x => avgConsensus).ToList();
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
