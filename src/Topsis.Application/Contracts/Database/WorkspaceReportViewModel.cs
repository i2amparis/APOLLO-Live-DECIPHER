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
            UserId = user.UserId;
            var analysis = workspace.GetReportData();
            ChartAlternatives = BuildAlternativesChartData(workspace, analysis, user).ToArray();
            ChartConsensus = analysis?.GroupConsensus;
            ChartGroups = analysis?.GroupTopsis;
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

            var groupAlternatives = analysis.GroupTopsis[StakeholderTopsis.DefaultGroupName]
                .ToDictionary(x => x.AlternativeId, x => x.Topsis);

            foreach (var g in analysis.StakeholderTopsis.GroupBy(x => x.AlternativeId))
            {
                var votes = g.ToArray();
                var stakeHolderTopsis = votes.FirstOrDefault(x => x.StakeholderId == user?.UserId)?.Topsis;
                var alternative = alternativesDict[g.Key];

                groupAlternatives.TryGetValue(g.Key, out var groupTopsis);
                yield return new AlternativeChartItem(alternative, stakeHolderTopsis, groupTopsis);
            }
        }

        public Workspace Workspace { get; set; }
        public string UserId { get; }
        public AlternativeChartItem[] ChartAlternatives { get; set; }
        public IDictionary<string, double> ChartConsensus { get; set; }
        public Dictionary<string, AlternativeTopsis[]> ChartGroups { get; }

        public class AlternativeChartItem
        {
            public AlternativeChartItem(string alternative, double? stakeholderTopsis, double groupTopsis)
            {
                Alternative = alternative;
                StakeholderTopsis = stakeholderTopsis ?? 0;
                GroupTopsis = groupTopsis;
            }

            [JsonPropertyName("alt")]
            [JsonProperty("alt")]
            public string Alternative { get; }
            [JsonPropertyName("mytopsis")]
            [JsonProperty("mytopsis")]
            public double StakeholderTopsis { get; }
            [JsonPropertyName("grouptopsis")]
            [JsonProperty("grouptopsis")]
            public double GroupTopsis { get; }
        }
    }

}
