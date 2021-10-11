using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Topsis.Domain;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Contracts.Database
{
    public class WorkspaceReportViewModel
    {
        public WorkspaceReportViewModel(Workspace workspace, IUserContext user)
        {
            Workspace = workspace;
            var analysis = workspace.GetReportData();
            ChartAlternatives = BuildAlternativesChartData(workspace, analysis, user).ToArray();
            ChartConsensus = analysis?.GroupConsensus;
        }

        private IEnumerable<AlternativeChartItem> BuildAlternativesChartData(Workspace workspace, 
            WorkspaceAnalysisResult analysis, 
            IUserContext user)
        {
            if (analysis == null)
            {
                yield break;
            }

            var alternativesDict = workspace.Questionnaire.Alternatives.ToDictionary(x => x.Id, x => x.Title);

            foreach (var g in analysis.StakeholderAnswers.GroupBy(x => x.AlternativeId))
            {
                var votes = g.ToArray();
                var avgTopsis = votes.Select(x => x.MyTopsis).Average();
                var stakeHolderTopsis = votes.FirstOrDefault(x => x.StakeholderId == user?.UserId)?.MyTopsis;
                var alternative = alternativesDict[g.Key];
                yield return new AlternativeChartItem(alternative, stakeHolderTopsis, avgTopsis);
            }
        }

        public Workspace Workspace { get; set; }
        public AlternativeChartItem[] ChartAlternatives { get; set; }
        public IDictionary<string, double> ChartConsensus { get; set; }

        public class AlternativeChartItem
        {
            public AlternativeChartItem(string alternative, double? stakeholderTopsis, double averageTopsis)
            {
                Alternative = alternative;
                StakeholderTopsis = stakeholderTopsis ?? 0;
                AverageTopsis = averageTopsis;
            }

            [JsonPropertyName("alt")]
            [JsonProperty("alt")]
            public string Alternative { get; }
            [JsonPropertyName("mytopsis")]
            [JsonProperty("mytopsis")]
            public double StakeholderTopsis { get; }
            [JsonPropertyName("grouptopsis")]
            [JsonProperty("grouptopsis")]
            public double AverageTopsis { get; }
        }
    }

}
