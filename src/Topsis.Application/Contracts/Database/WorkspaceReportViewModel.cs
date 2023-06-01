using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Topsis.Domain;
using Topsis.Domain.Common;
using Topsis.Domain.Contracts;

namespace Topsis.Application.Contracts.Database
{
    public class WorkspaceReportViewModel
    {
        private const int MaxNumberOfTipsCount = 1;

        public WorkspaceReportViewModel(Workspace workspace, IUserContext user)
        {
            Workspace = workspace;
            UserId = user.UserId;
            var report = workspace.GetReportData();
            HasUserVoted = workspace.Votes.Any(x => x.ApplicationUserId == UserId);
            ChartAlternatives = BuildAlternativesChartData(workspace, report, user).ToArray();
            ChartConsensus = report?.StakeholdersConsensus;
            ChartGroups = report?.GroupTopsis;
            AlternativesConsensusDegree = report?.AlternativesConsensusDegree;

            double myConsensus = 0;
            if (ChartConsensus?.TryGetValue(UserId, out myConsensus) == true)
            {
                MyConsensus = Rounder.Round(100d * myConsensus, 1);
            }

            if (ChartConsensus?.Values.Any() == true)
            {
                AverageConsensus = Rounder.Round(100d * ChartConsensus.Values.Average(), 1);
            }

            Tips = BuildTips(report, workspace).OrderByDescending(x => x.Distance).Take(MaxNumberOfTipsCount).ToArray();

            if (workspace.Reports.Count > 1)
            {
                ReportComparison = BuildReportComparisonChartData(workspace);
            }
        }

        private IDictionary<FeedbackRound, AlternativeChartItem[]> BuildReportComparisonChartData(Workspace workspace)
        {
            var result = new Dictionary<FeedbackRound, AlternativeChartItem[]>();
            foreach (var report in workspace.Reports.OrderByDescending(x => x.CreatedAtUtc))
            {
                var alternatives = BuildAlternativesChartData(workspace, report.GetAnalysisResult(), null).ToArray();
                result[report.Round] = alternatives;
            }

            return result;
        }

        private IEnumerable<AlternativeChartItem> BuildAlternativesChartData(Workspace workspace, 
            WorkspaceAnalysisResult analysis, 
            IUserContext user)
        {
            if (analysis == null)
            {
                yield break;
            }

            var alternativesDict = workspace.Questionnaire.AlternativesDictionary;

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
        public bool HasUserVoted { get; }
        public AlternativeChartItem[] ChartAlternatives { get; set; }
        public IDictionary<FeedbackRound, AlternativeChartItem[]> ReportComparison { get; }
        public IDictionary<string, double> ChartConsensus { get; set; }
        public Dictionary<string, AlternativeTopsis[]> ChartGroups { get; }
        public IDictionary<int, double> AlternativesConsensusDegree { get; }
        public double MyConsensus { get; }
        public double AverageConsensus { get; }
        public IList<FeedbackTip> Tips { get; }

        private IEnumerable<FeedbackTip> BuildTips(WorkspaceAnalysisResult report, Workspace workspace)
        {
            if (workspace.CanProvideTips() == false)
            {
                yield break;
            }

            if (report == null || MyConsensus == 0 || AverageConsensus == 0 || MyConsensus > AverageConsensus)
            {
                // user doesn't have any tips.
                yield break;
            }

            // user consensus is below AVG.
            AlternativeTopsis[] globalTopsis = null;
            if (report.GroupTopsis?.TryGetValue(StakeholderTopsis.DefaultGroupName, out globalTopsis) == false)
            {
                yield break;
            }

            var globalTopsisDict = globalTopsis.ToDictionary(x => x.AlternativeId, x => x.Topsis);
            foreach (var item in report.StakeholderTopsis.Where(x => string.Equals(x.StakeholderId, UserId, StringComparison.OrdinalIgnoreCase)))
            {
                if (globalTopsisDict.TryGetValue(item.AlternativeId, out var globalAltTopsis)
                    && workspace.Questionnaire.AlternativesDictionary.TryGetValue(item.AlternativeId, out var alternative))
                {
                    yield return new FeedbackTip(item, globalAltTopsis, alternative.Title);
                }
            }
        }

        public class AlternativeChartItem
        {
            public AlternativeChartItem(Alternative alternative, double? stakeholderTopsis, double groupTopsis)
            {
                AlternativeTitle = alternative.Title;
                AlternativeOrder = alternative.Order;
                StakeholderTopsis = stakeholderTopsis ?? 0;
                GroupTopsis = groupTopsis;
            }

            [JsonPropertyName("at")]
            [JsonProperty("at")]
            public string AlternativeTitle { get; }
            [JsonPropertyName("ao")]
            [JsonProperty("ao")]
            public short AlternativeOrder { get; }
            [JsonPropertyName("mytopsis")]
            [JsonProperty("mytopsis")]
            public double StakeholderTopsis { get; }
            [JsonPropertyName("grouptopsis")]
            [JsonProperty("grouptopsis")]
            public double GroupTopsis { get; }
        }
    }
}
