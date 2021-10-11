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
        public Task<WorkspaceAnalysisResult> AnalyzeAsync(Workspace workspace, IList<StakeholderAnswerDto> answers)
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
                var stakeholderTopsis = GetAlternativesResults(s.Key, distancesTable);
                result.AddStakeholderAnalysis(stakeholderTopsis);
            }

            // Group Consensus.
            var consensus = new TopsisConsensus();
            var settings = workspace.Questionnaire.GetSettings();
            var stakeholdersConsent = consensus.Consent(settings, result.StakeholderAnswers);

            result.AddGroupAnalysis(stakeholdersConsent);

            return Task.FromResult(result);
        }

        private IEnumerable<WorkspaceAnalysisResultItem> GetAlternativesResults(string stakeholderId, DataTable distancesTable)
        {
            foreach (DataRow row in distancesTable.Rows)
            {
                var alternativeId = (int)row[ColumnHelper.GetAlternativeColumnName()];
                var topsis = (double)row[ColumnHelper.GetTopsisColumnName()];
                yield return WorkspaceAnalysisResultItem.Create(stakeholderId, alternativeId, topsis);
            }
        }
    }

    internal static class ColumnHelper
    {
        internal static string GetCriterionColumnName(int criterionId)
        {
            return $"c_{criterionId}";
        }

        internal static string GetAlternativeColumnName()
        {
            return "alternative";
        }

        internal static string GetDistanceColumnName(bool isPositive)
        {
            var suffix = isPositive ? "positive" : "negative";
            return $"distance_{suffix}";
        }

        internal static string GetCriterionDistanceColumnName(int criterionId, bool isPositive)
        {
            var suffix = isPositive ? "positive" : "negative";
            return $"c_{criterionId}_{suffix}";
        }

        internal static string GetTopsisColumnName()
        {
            return "topsis";
        }
    }
}
