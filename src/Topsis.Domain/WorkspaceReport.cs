using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Topsis.Domain.Common;

namespace Topsis.Domain
{
    public enum AlgorithmType : short
    { 
        Topsis = 0
    }

    public class WorkspaceReport : Entity 
    {
        private string _analysisResultJson;

        public WorkspaceReport()
        {
            CreatedAtUtc = DateTime.UtcNow;
            VotesCount = 0;
            Criteria = string.Empty;
            Alternatives = string.Empty;
            Algorithm = AlgorithmType.Topsis;
        }

        public Workspace Workspace { get; set; }
        public int WorkspaceId { get; set; }
        public AlgorithmType Algorithm { get; set; }

        public int VotesCount { get; set; }
        public string Criteria { get; set; }
        public string Alternatives { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }

        public void InitializeFrom(Workspace workspace)
        {
            CompletedAtUtc = null;
            VotesCount = workspace.Votes.Count;
            Criteria = string.Join(",", workspace.Questionnaire.Criteria.Select(x => x.Title).ToArray());
            Alternatives = string.Join(",", workspace.Questionnaire.Alternatives.Select(x => x.Title).ToArray());
        }

        public WorkspaceAnalysisResult GetAnalysisResult()
        {
            if (CompletedAtUtc.HasValue == false)
            {
                return null;
            }

            return Serializer.DeserializeFromJson<WorkspaceAnalysisResult>(_analysisResultJson);
        }

        public static WorkspaceReport Create(Workspace workspace, AlgorithmType algorithm)
        {
            var result = new WorkspaceReport
            {
                WorkspaceId = workspace.Id,
                CreatedAtUtc = DateTime.UtcNow,
                Algorithm = algorithm
            };

            result.InitializeFrom(workspace);
            return result;
        }

        internal void SetResult(WorkspaceAnalysisResult result)
        {
            _analysisResultJson = Serializer.SerializeToJson(result);
            CompletedAtUtc = DateTime.UtcNow;
        }
    }

    public class WorkspaceAnalysisResult
    {
        public WorkspaceAnalysisResult()
        {
            StakeholderAnswers = new List<WorkspaceAnalysisResultItem>();
        }

        [JsonPropertyName("items")]
        [JsonProperty("items")]
        public List<WorkspaceAnalysisResultItem> StakeholderAnswers { get; set; }

        [JsonPropertyName("consensus")]
        [JsonProperty("consensus")]
        public IDictionary<string, double> GroupConsensus { get; set; }

        public void AddGroupAnalysis(IDictionary<string, double> stakeholdersConsensus)
        {
            GroupConsensus = stakeholdersConsensus;
        }

        public void AddStakeholderAnalysis(IEnumerable<WorkspaceAnalysisResultItem> stakeholderTopsis)
        {
            StakeholderAnswers.AddRange(stakeholderTopsis);
        }
    }

    public class WorkspaceAnalysisResultItem
    {
        [JsonPropertyName("uid")]
        [JsonProperty("uid")]
        public string StakeholderId { get; set; }
        [JsonPropertyName("aid")]
        [JsonProperty("aid")]
        public int AlternativeId { get; set; }
        [JsonPropertyName("mytopsis")]
        [JsonProperty("mytopsis")]
        public double MyTopsis { get; set; }
        [JsonPropertyName("dis")]
        [JsonProperty("dis")]
        public double Dissimilarity { get; set; }

        public static WorkspaceAnalysisResultItem Create(string stakeholderId,
            int alternativeId,
            double result)
        {
            return new WorkspaceAnalysisResultItem
            {
                AlternativeId = alternativeId,
                StakeholderId = stakeholderId,
                MyTopsis = Rounder.Round(result)
            };
        }

        public void SetDissimilarity(double dissimilarity)
        {
            Dissimilarity = Rounder.Round(dissimilarity);
        }
    }
}
