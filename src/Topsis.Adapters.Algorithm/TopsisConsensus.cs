using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Topsis.Domain;
using Topsis.Domain.Common;

namespace Topsis.Adapters.Algorithm
{
    internal class TopsisConsensus
    {
        public IDictionary<string,double> Consent(QuestionnaireSettings settings, IList<Domain.WorkspaceAnalysisResultItem> stakeholderAnswers)
        {
            // Calculate topsis global - average of stakeholder alternatives topsis
            var globalTopsis = CalculateGlobalTopsis(stakeholderAnswers);

            // Consensus
            var stakeholdersConsensus = new Dictionary<string, double>();
            CalculateDissimilarity(settings, globalTopsis, stakeholderAnswers, stakeholdersConsensus);

            return stakeholdersConsensus;
        }

        private void CalculateDissimilarity(QuestionnaireSettings settings,
            IDictionary<int, double> globalTopsisAverage, 
            IList<WorkspaceAnalysisResultItem> stakeholderAnswers,
            IDictionary<string, double> stakeholderTotalConsensus)
        {
            var globalTopsisSum = globalTopsisAverage.Values.Sum();

            foreach (var g in stakeholderAnswers.GroupBy(x => x.StakeholderId))
            {
                // =SUMPRODUCT(Data!$D$5:$D$18*Dissimilarity!E26:E39)/SUM(Data!$D$5:$D$18)
                // sumproduct(globalTopsis*stakeholderDissimilarity)/sum(globalTopsis)
                double productSum = 0;

                foreach (var stakeholderAlt in g.ToArray())
                {
                    // Calculate dissimilarity.
                    // =(ABS(Data!$D5-Data!E5)/($B$4-1))^$B$3
                    // =(ABS(GlobalTopsis-StakeholderTopsis)/(OutputLinquisticScale))^Rigorousness

                    var globalTopsis = globalTopsisAverage[stakeholderAlt.AlternativeId];
                    var dissimilarity = 1 - Math.Pow(Math.Abs(globalTopsis - stakeholderAlt.MyTopsis) / (double)settings.Scale, settings.Rigorousness);
                    stakeholderAlt.SetDissimilarity(dissimilarity);

                    productSum += globalTopsis * dissimilarity;
                }

                stakeholderTotalConsensus[g.Key] = Rounder.Round(productSum / globalTopsisSum);
            }
        }

        #region [ Helpers ]
        private IDictionary<int, double> CalculateGlobalTopsis(IList<WorkspaceAnalysisResultItem> stakeholderAnswers)
        {
            var result = new Dictionary<int, double>();
            foreach (var g in stakeholderAnswers.GroupBy(x => x.AlternativeId))
            {
                result[g.Key] = g.Select(x => x.MyTopsis).Average();
            }

            return result;
        }
        #endregion
    }
}
