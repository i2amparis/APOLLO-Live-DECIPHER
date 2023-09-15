using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Domain;
using Topsis.Domain.Common;

namespace Topsis.Adapters.Algorithm
{
    public class TopsisAnalyzer : ITopsisAlgorithm
    {
        public Task<WorkspaceAnalysisResult> AnalyzeAsync(Workspace workspace, 
            IDictionary<int, string> jobCategories, 
            IList<StakeholderAnswerDto> answers,
            IList<StakeholderDemographicsDto> stakeholdersDemographics)
        {
            stakeholdersDemographics ??= new StakeholderDemographicsDto[0];

            var normalizer = new TopsisNormalizer();
            var distance = new TopsisDistanceCalculator();

            var result = new WorkspaceAnalysisResult();
            var stakeholders = answers.GroupBy(x => x.StakeholderId);
            var stakeholderWeights = BuildStakeholderWeights(workspace);

            foreach (var s in stakeholders)
            {
                var stakeholderAnswers = s.ToArray();

                var normalizedTable = normalizer.Normalize(workspace, stakeholderAnswers);
                var distancesTable = distance.Calculate(workspace, normalizedTable);
                var stakeholderWeight = stakeholderWeights.TryGetValue(s.Key, out var weight) ? weight : 1;
                var stakeholderTopsis = GetAlternativesResults(s.Key, stakeholderAnswers.First().JobCategoryId, distancesTable, stakeholderWeight);
                result.AddStakeholderAnalysis(stakeholderTopsis);
            }

            var settings = workspace.Questionnaire.GetSettings();

            // Consensus Degree.

            var globalTopsis = AddGroupTopsis(result, settings, jobCategories, stakeholdersDemographics);
            AddGroupConsensus(result, settings, globalTopsis);

            return Task.FromResult(result);
        }

        private Dictionary<string,double> BuildStakeholderWeights(Workspace workspace)
        {
            return workspace.Votes.GroupBy(x => x.ApplicationUserId).ToDictionary(x => x.Key, x => x.OrderByDescending(x => x.Id).First().Weight ?? 1);
        }

        private static void AddGroupConsensus(WorkspaceAnalysisResult result, QuestionnaireSettings settings, IDictionary<int, double> globalTopsis)
        {
            var consensus = new TopsisConsensus();
            var (stakeholdersConsent, consensusDegree) = consensus.Calculate(settings, result.StakeholderTopsis, globalTopsis);
            result.AddConsensusAnalysis(stakeholdersConsent, consensusDegree);
        }

        /// <summary>
        /// Returns the global topsis key:alternativeId-value:topsis.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="settings"></param>
        /// <param name="jobCategories"></param>
        /// <param name="answers"></param>
        /// <returns></returns>
        private static IDictionary<int, double> AddGroupTopsis(WorkspaceAnalysisResult result, 
            QuestionnaireSettings settings, 
            IDictionary<int, string> jobCategories, 
            IList<StakeholderDemographicsDto> stakeholders)
        {
            var groupTopsis = new GroupTopsis(settings, result.StakeholderTopsis);
            var alternativeGroupItems = groupTopsis.Calculate().ToArray();
            result.AddGroupSolution(alternativeGroupItems);

            var stakeholdersWithJob = stakeholders.Where(x => x.JobCategoryId.HasValue).ToArray();
            foreach (var jobCategory in stakeholdersWithJob.GroupBy(x => x.JobCategoryId.Value))
            {
                if (jobCategories.TryGetValue(jobCategory.Key, out var title))
                {
                    var userIds = jobCategory.Select(x => x.Id).ToArray();
                    var subGroupAnswers = result.StakeholderTopsis.Where(x => userIds.Contains(x.StakeholderId)).ToList();
                    var voteCount = subGroupAnswers.GroupBy(x => x.StakeholderId).Count();
                    var alternativeSubgroupItems = CalculateSubgroupTopsis(settings, subGroupAnswers, voteCount);
                    result.AddGroupSolution(alternativeSubgroupItems, $"{title} ({voteCount})");
                }
            }

            var stakeholdersWithGender = stakeholders.Where(x => x.GenderId.HasValue).ToArray();
            if (stakeholdersWithGender.Any())
            {
                foreach (var genderGroup in stakeholdersWithGender.GroupBy(x => x.GenderId))
                {
                    var userIds = genderGroup.Select(x => x.Id).ToArray();
                    var subGroupAnswers = result.StakeholderTopsis.Where(x => userIds.Contains(x.StakeholderId)).ToList();
                    var voteCount = subGroupAnswers.GroupBy(x => x.StakeholderId).Count();
                    var alternativeSubgroupItems = CalculateSubgroupTopsis(settings, subGroupAnswers, voteCount);
                    
                    var title = genderGroup.Key.GetDescription();
                    result.AddGroupSolution(alternativeSubgroupItems, $"g:{title} ({voteCount})");
                }
            }

            return alternativeGroupItems.ToDictionary(x => x.AlternativeId, x => x.Topsis);
        }

        private static AlternativeTopsis[] CalculateSubgroupTopsis(QuestionnaireSettings settings, List<StakeholderTopsis> subGroupAnswers, int voteCount)
        {
            if (voteCount == 1)
            {
                // discussion: 3/3/2022
                // When group has only one stakeholder then return existing stakeholder topsis.
                return subGroupAnswers.Select(x => new AlternativeTopsis { AlternativeId = x.AlternativeId, Topsis = x.Topsis }).ToArray();
            }

            var subGroupTopsis = new GroupTopsis(settings, subGroupAnswers);
            return subGroupTopsis.Calculate().ToArray();
        }

        private IEnumerable<StakeholderTopsis> GetAlternativesResults(string stakeholderId, int? jobCategoryId, DataTable distancesTable, double stakeholderWeight)
        {
            foreach (DataRow row in distancesTable.Rows)
            {
                var alternativeId = (int)row[ColumnHelper.GetAlternativeColumnName()];
                var topsis = (double)row[ColumnHelper.GetTopsisColumnName()];
                yield return StakeholderTopsis.Create(stakeholderId, alternativeId, topsis, jobCategoryId, stakeholderWeight);
            }
        }
    }
}
