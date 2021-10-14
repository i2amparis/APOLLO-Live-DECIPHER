using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Algorithm;
using Topsis.Domain;

namespace Topsis.Adapters.Algorithm
{
    public class TopsisAnalyzer : ITopsisAlgorithm
    {
        public Task<WorkspaceAnalysisResult> AnalyzeAsync(Workspace workspace, 
            IDictionary<int, string> jobCategories, 
            IList<StakeholderAnswerDto> answers)
        {
            var normalizer = new TopsisNormalizer();
            var distance = new TopsisDistanceCalculator();

            var result = new WorkspaceAnalysisResult();
            var stakeholders = answers.GroupBy(x => x.StakeholderId);

            foreach (var s in stakeholders)
            {
                var stakeholderAnswers = s.ToArray();

                var normalizedTable = normalizer.Normalize(workspace, stakeholderAnswers);
                var distancesTable = distance.Calculate(workspace, normalizedTable);
                var stakeholderTopsis = GetAlternativesResults(s.Key, stakeholderAnswers.First().JobCategoryId, distancesTable);
                result.AddStakeholderAnalysis(stakeholderTopsis);
            }

            var settings = workspace.Questionnaire.GetSettings();

            // Consensus Degree.

            AddGroupTopsis(result, settings, jobCategories, answers);
            AddGroupConsensus(result, settings);

            return Task.FromResult(result);
        }

        private static void AddGroupConsensus(WorkspaceAnalysisResult result, QuestionnaireSettings settings)
        {
            var consensus = new TopsisConsensus();
            var (stakeholdersConsent, consensusDegree) = consensus.Calculate(settings, result.StakeholderTopsis);
            result.AddConsensusAnalysis(stakeholdersConsent, consensusDegree);
        }

        private static void AddGroupTopsis(WorkspaceAnalysisResult result, 
            QuestionnaireSettings settings, 
            IDictionary<int, string> jobCategories, 
            IList<StakeholderAnswerDto> answers)
        {
            var groupTopsis = new GroupTopsis(settings, result.StakeholderTopsis);
            var alternativeGroupItems = groupTopsis.Calculate().ToArray();
            result.AddGroupSolution(alternativeGroupItems);

            foreach (var jobCategoryId in answers.Select(x => x.JobCategoryId).Distinct().Where(x => x.HasValue).Select(x => x.Value))
            {
                if (jobCategories.TryGetValue(jobCategoryId, out var title))
                {
                    var subGroupAnswers = result.StakeholderTopsis.Where(x => x.JobCategoryId == jobCategoryId).ToList();
                    var subGroupTopsis = new GroupTopsis(settings, subGroupAnswers);
                    var alternativeSubgroupItems = subGroupTopsis.Calculate().ToArray();
                    var voteCount = subGroupAnswers.GroupBy(x => x.StakeholderId).Count();
                    result.AddGroupSolution(alternativeSubgroupItems, $"{title} ({voteCount})");
                }
            }
        }

        private IEnumerable<StakeholderTopsis> GetAlternativesResults(string stakeholderId, int? jobCategoryId, DataTable distancesTable)
        {
            foreach (DataRow row in distancesTable.Rows)
            {
                var alternativeId = (int)row[ColumnHelper.GetAlternativeColumnName()];
                var topsis = (double)row[ColumnHelper.GetTopsisColumnName()];
                yield return StakeholderTopsis.Create(stakeholderId, alternativeId, topsis, jobCategoryId);
            }
        }
    }
}
