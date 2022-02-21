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
            IList<StakeholderTopsis> stakeholderTopsis,
            IDictionary<int, double> globalTopsis)
        {
            // Consensus
            var globalTopsisSum = globalTopsis.Values.Sum();

            var alternativeDissimilarities = globalTopsis.ToDictionary(x => x.Key, x => new List<double>());
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

                    var altGlobalTopsis = globalTopsis[stakeholderAlt.AlternativeId];
                    var dissimilarity = Math.Pow(Math.Abs(altGlobalTopsis - stakeholderAlt.Topsis) / (double)settings.Scale, settings.Rigorousness);

                    if (alternativeDissimilarities.TryGetValue(stakeholderAlt.AlternativeId, out var altDissimilarities))
                    {
                        altDissimilarities.Add(dissimilarity);
                    }

                    // Global Consensus
                    productSum += altGlobalTopsis * (1 - dissimilarity);
                }

                stakeholderTotalConsensus[g.Key] = Rounder.Round(productSum / globalTopsisSum);
            }

            // Consensus Degree: =1-AVERAGE(E5:T5)
            var consensusDegree = alternativeDissimilarities.ToDictionary(x => x.Key, x => Rounder.Round(1 - x.Value.Average()));

            return (stakeholderTotalConsensus, consensusDegree);
        }
    }
}
