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
        public (IDictionary<string, double>, IDictionary<int, double>) Calculate(QuestionnaireSettings settings, 
            IList<StakeholderTopsis> stakeholderAnswers,
            IDictionary<int, double> globalTopsis)
        {
            // Calculate topsis global - average of stakeholder alternatives topsis
            //var globalTopsis = new GroupTopsis(settings, stakeholderAnswers).Calculate()

            // Consensus
            return CalculateDissimilarity(settings, globalTopsis, stakeholderAnswers);
        }

        private (IDictionary<string,double>, IDictionary<int,double>) CalculateDissimilarity(
            QuestionnaireSettings settings,
            IDictionary<int, double> globalTopsisAverage, 
            IList<StakeholderTopsis> stakeholderTopsis)
        {
            var globalTopsisSum = globalTopsisAverage.Values.Sum();

            var alternativeDissimilarities = globalTopsisAverage.ToDictionary(x => x.Key, x => new List<double>());
            var stakeholderTotalConsensus = new Dictionary<string, double>();

            foreach (var g in stakeholderTopsis.GroupBy(x => x.StakeholderId))
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
                    var dissimilarity = Math.Pow(Math.Abs(globalTopsis - stakeholderAlt.Topsis) / (double)settings.Scale, settings.Rigorousness);

                    if (alternativeDissimilarities.TryGetValue(stakeholderAlt.AlternativeId, out var altDissimilarities))
                    {
                        altDissimilarities.Add(dissimilarity);
                    }

                    // Global Consensus
                    productSum += globalTopsis * (1 - dissimilarity);
                }

                stakeholderTotalConsensus[g.Key] = Rounder.Round(productSum / globalTopsisSum);
            }

            // Consensus Degree: =1-AVERAGE(E5:T5)
            var consensusDegree = alternativeDissimilarities.ToDictionary(x => x.Key, x => Rounder.Round(1 - x.Value.Average()));

            return (stakeholderTotalConsensus, consensusDegree);
        }

        #region [ Helpers ]
        private IDictionary<int, double> CalculateGlobalTopsis(IList<StakeholderTopsis> stakeholderAnswers)
        {
            var result = new Dictionary<int, double>();
            foreach (var g in stakeholderAnswers.GroupBy(x => x.AlternativeId))
            {
                result[g.Key] = g.Select(x => x.Topsis).Average();
            }

            return result;
        }
        #endregion
    }
}
