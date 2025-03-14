﻿using Newtonsoft.Json;
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

    public enum FeedbackRound : short 
    {
        Undefined = 0,
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4,
        Fifth = 5,
        Sixth = 6,
        Seventh = 7,
        Eighth = 8,
        Ninth = 9,
        Tenth = 10
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
            Round = FeedbackRound.First;
        }

        public Workspace Workspace { get; set; }
        public int WorkspaceId { get; set; }
        public AlgorithmType Algorithm { get; set; }
        public FeedbackRound Round { get; set; }
        public int VotesCount { get; set; }
        public string Criteria { get; set; }
        public string Alternatives { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }

        public void InitializeFrom(Workspace workspace)
        {
            CompletedAtUtc = null;
            VotesCount = workspace.Votes.GroupBy(x => x.ApplicationUserId).Count();
            Criteria = string.Join(",", workspace.Questionnaire.Criteria.OrderBy(x => x.Order).Select(x => x.Title).ToArray());
            Alternatives = string.Join(",", workspace.Questionnaire.Alternatives.OrderBy(x => x.Order).Select(x => x.Title).ToArray());
        }

        public WorkspaceAnalysisResult GetAnalysisResult()
        {
            if (CompletedAtUtc.HasValue == false)
            {
                return null;
            }

            return Serializer.DeserializeFromJson<WorkspaceAnalysisResult>(_analysisResultJson);
        }

        public static WorkspaceReport Create(Workspace workspace, FeedbackRound round)
        {
            var result = new WorkspaceReport
            {
                WorkspaceId = workspace.Id,
                CreatedAtUtc = DateTime.UtcNow,
                Algorithm = AlgorithmType.Topsis,
                Round = round
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
            StakeholderTopsis = new List<StakeholderTopsis>();
            GroupTopsis = new Dictionary<string, AlternativeTopsis[]>();
        }

        [JsonPropertyName("stakeholders")]
        [JsonProperty("stakeholders")]
        public List<StakeholderTopsis> StakeholderTopsis { get; set; }

        [JsonPropertyName("groups")]
        [JsonProperty("groups")]
        public Dictionary<string, AlternativeTopsis[]> GroupTopsis { get; set; }

        [JsonPropertyName("consensus_stakeholders")]
        [JsonProperty("consensus_stakeholders")]
        public IDictionary<string, double> StakeholdersConsensus { get; set; }

        [JsonPropertyName("consensus_degree")]
        [JsonProperty("consensus_degree")]
        public IDictionary<int, double> AlternativesConsensusDegree { get; set; }

        public void AddConsensusAnalysis(IDictionary<string, double> stakeholdersConsensus, 
            IDictionary<int, double> consensusDegree)
        {
            StakeholdersConsensus = stakeholdersConsensus;
            AlternativesConsensusDegree = consensusDegree;
        }

        public void AddGroupSolution(AlternativeTopsis[] groupTopsis, string groupName = null)
        {
            GroupTopsis[groupName ?? Domain.StakeholderTopsis.DefaultGroupName] = groupTopsis;
        }

        public void AddStakeholderAnalysis(IEnumerable<StakeholderTopsis> stakeholderTopsis)
        {
            StakeholderTopsis.AddRange(stakeholderTopsis);
        }
    }

    public class AlternativeTopsis
    { 
        [JsonProperty("topsis")]
        [JsonPropertyName("topsis")]
        public double Topsis { get; set; }

        [JsonPropertyName("aid")]
        [JsonProperty("aid")]
        public int AlternativeId { get; set; }
    }
}
