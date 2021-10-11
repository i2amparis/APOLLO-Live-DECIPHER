using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Topsis.Domain;
using Topsis.Domain.Common;

namespace Topsis.Adapters.Algorithm
{
    internal class TopsisDistanceCalculator
    {
        public DataTable Calculate(Workspace workspace, DataTable normalizedTable)
        {
            var criteria = workspace.Questionnaire.Criteria.ToDictionary(x => x.Id, x => x);

            return BuildDistancesTable(workspace.Questionnaire.GetSettings(), criteria, normalizedTable);
        }

        private DataTable BuildDistancesTable(QuestionnaireSettings settings,
            Dictionary<int, Criterion> criteria,
            DataTable normalizedTable)
        {
            DataTable result = BuildDistancesTableSchema(criteria);

            var positiveIdeals = BuildIdeals(criteria, normalizedTable, true);
            var negativeIdeals = BuildIdeals(criteria, normalizedTable, false);

            // ABS(J4-J$21)
            // ABS(normalizedValue-ideal)
            foreach (DataRow normalizedRow in normalizedTable.Rows)
            {
                var distanceRow = result.NewRow();
                distanceRow[ColumnHelper.GetAlternativeColumnName()] = normalizedRow[ColumnHelper.GetAlternativeColumnName()];

                double sumOfNegative = 0;
                double sumOfPositive = 0;

                foreach (var item in criteria)
                {
                    var columnName = ColumnHelper.GetCriterionColumnName(item.Key);
                    var normalizedValue = (double)normalizedRow[columnName];

                    foreach (var isPositive in new[] { true, false })
                    {
                        // find ideal value.
                        var ideals = isPositive ? positiveIdeals : negativeIdeals;
                        var ideal = isPositive ? ideals.Values.Max() : ideals.Values.Min();

                        var distanceColumnName = ColumnHelper.GetCriterionDistanceColumnName(item.Key, isPositive);
                        var distance = Math.Abs(normalizedValue - ideal);
                        distanceRow[distanceColumnName] = distance;
                        sumOfNegative += isPositive ? 0 : distance;
                        sumOfPositive += isPositive ? distance : 0;
                    }
                }

                var criteriaCount = criteria.Count;
                var positiveDistance = sumOfPositive / criteriaCount;
                distanceRow[ColumnHelper.GetDistanceColumnName(true)] = positiveDistance;

                var negativeDistance = sumOfNegative / criteriaCount;
                distanceRow[ColumnHelper.GetDistanceColumnName(false)] = negativeDistance;

                // 7-scale Dist: 6 * (AF4/(AE4+AF4)) | 6*(NegativeDistance/(PositiveDistance+NegativeDistance))
                var scale = (short)settings.Scale;
                var topsis = 0d;
                if (positiveDistance + negativeDistance != 0)
                {
                    topsis = Rounder.Round(scale * (negativeDistance / (positiveDistance + negativeDistance)));
                }

                distanceRow[ColumnHelper.GetTopsisColumnName()] = topsis;

                result.Rows.Add(distanceRow);
            }

            return result;
        }

        private static DataTable BuildDistancesTableSchema(Dictionary<int, Criterion> criteria)
        {
            var result = new DataTable();
            result.Columns.Add(ColumnHelper.GetAlternativeColumnName(), typeof(int));

            // build positive/negative criteria columns
            foreach (var kvp in criteria)
            {
                result.Columns.Add(ColumnHelper.GetCriterionDistanceColumnName(kvp.Key, true), typeof(double));
                result.Columns.Add(ColumnHelper.GetCriterionDistanceColumnName(kvp.Key, false), typeof(double));
            }

            // build positive and negative columns
            result.Columns.Add(ColumnHelper.GetDistanceColumnName(true), typeof(double));
            result.Columns.Add(ColumnHelper.GetDistanceColumnName(false), typeof(double));

            // build topsis column.
            result.Columns.Add(ColumnHelper.GetTopsisColumnName(), typeof(double));

            return result;
        }

        private Dictionary<int, double> BuildIdeals(Dictionary<int, Criterion> criteria, DataTable normalizedTable, bool isPositive)
        {
            var result = new Dictionary<int, double>();

            foreach (var kvp in criteria)
            {
                var values = normalizedTable.Rows.OfType<DataRow>().Select(x => (double)x[ColumnHelper.GetCriterionColumnName(kvp.Key)]).ToArray();
                result[kvp.Key] = kvp.Value.GetIdeal(isPositive, values);
            }

            return result;
        }
    }
}
