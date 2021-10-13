using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Topsis.Domain;
using Topsis.Domain.Common;

namespace Topsis.Adapters.Algorithm
{
    public class GroupTopsis
    {
        private readonly QuestionnaireSettings _settings;
        private Dictionary<string, StakeholderTopsis[]> _stakeholders;
        private Dictionary<int, StakeholderTopsis[]> _alternatives;

        public GroupTopsis(QuestionnaireSettings settings,
            List<StakeholderTopsis> stakeholderAnswers)
        {
            _stakeholders = stakeholderAnswers.GroupBy(x => x.StakeholderId).ToDictionary(x => x.Key, x => x.ToArray());
            _alternatives = stakeholderAnswers.GroupBy(x => x.AlternativeId).ToDictionary(x => x.Key, x => x.ToArray());
            _settings = settings;
        }

        internal IEnumerable<AlternativeTopsis> Calculate()
        {
            // every stakeholder answer vector becomes a "criterion" for topsis

            var normalizedTable = BuildNormalizedTable();
            var distanceTable = BuildDistanceTable(normalizedTable);
            foreach (DataRow row in distanceTable.Rows)
            {
                var alternativeId = row.Field<int>(ColumnHelper.GetAlternativeColumnName());
                var topsis = row.Field<double>(ColumnHelper.GetTopsisColumnName());
                yield return new AlternativeTopsis
                {
                    AlternativeId = alternativeId,
                    Topsis = topsis
                };
            }
        }

        private DataTable BuildDistanceTable(DataTable normalizedTable)
        {
            var result = BuildEmptyDistanceTable();

            // ideals.
            var stakeholderIdeals = _stakeholders.Keys.ToDictionary(x => x,
                                    x => normalizedTable.GetColumnMinMax(ColumnHelper.GetStakeholderColumnName(x)));

            var alternativeColumn = ColumnHelper.GetAlternativeColumnName();
            foreach (DataRow row in normalizedTable.Rows)
            {
                var newRow = result.NewRow();
                newRow[alternativeColumn] = row[alternativeColumn];

                double positiveDistanceSum = 0;
                double negativeDistanceSum = 0;
                foreach (var stakeholderId in _stakeholders.Keys)
                {
                    var stakeholderColumn = ColumnHelper.GetStakeholderColumnName(stakeholderId);
                    var stakeholderTopsis = row.Field<double>(stakeholderColumn);

                    var positiveColumn = ColumnHelper.GetStakeholderDistanceColumnName(stakeholderId, true);
                    var negativeColumn = ColumnHelper.GetStakeholderDistanceColumnName(stakeholderId, false);
                    var (min, max) = stakeholderIdeals[stakeholderId];
                    var posDistance = Math.Abs(max - stakeholderTopsis);
                    newRow[positiveColumn] = posDistance;
                    var negDistance = Math.Abs(min - stakeholderTopsis);
                    newRow[negativeColumn] = negDistance;

                    positiveDistanceSum += posDistance;
                    negativeDistanceSum += negDistance;
                }

                var positiveDistance = positiveDistanceSum / _stakeholders.Count;
                newRow[ColumnHelper.GetDistanceColumnName(true)] = positiveDistance;
                var negativeDistance = negativeDistanceSum / _stakeholders.Count;
                newRow[ColumnHelper.GetDistanceColumnName(false)] = negativeDistance;

                var scale = (short)_settings.Scale;
                var topsis = 0d;
                if (positiveDistance + negativeDistance != 0)
                {
                    topsis = Rounder.Round(scale * (negativeDistance / (positiveDistance + negativeDistance)));
                }

                newRow[ColumnHelper.GetTopsisColumnName()] = topsis;

                result.Rows.Add(newRow);
            }

            return result;
        }

        private DataTable BuildEmptyDistanceTable()
        {
            var result = new DataTable();

            // schema.
            var alternativeColumn = ColumnHelper.GetAlternativeColumnName();
            result.Columns.Add(alternativeColumn, typeof(int));
            var distancePositiveColumn = ColumnHelper.GetDistanceColumnName(true);
            result.Columns.Add(distancePositiveColumn, typeof(double));
            var distanceNegativeColumn = ColumnHelper.GetDistanceColumnName(false);
            result.Columns.Add(distanceNegativeColumn, typeof(double));
            result.Columns.Add(ColumnHelper.GetTopsisColumnName(), typeof(double));

            foreach (var stakeholderId in _stakeholders.Keys)
            {
                var positiveColumn = ColumnHelper.GetStakeholderDistanceColumnName(stakeholderId, true);
                var negativeColumn = ColumnHelper.GetStakeholderDistanceColumnName(stakeholderId, false);

                result.Columns.Add(positiveColumn, typeof(double));
                result.Columns.Add(negativeColumn, typeof(double));
            }

            return result;
        }

        private DataTable BuildNormalizedTable()
        {
            var result = new DataTable();

            // build schema.
            result.Columns.Add(ColumnHelper.GetAlternativeColumnName());
            foreach (var item in _stakeholders)
            {
                result.Columns.Add(ColumnHelper.GetStakeholderColumnName(item.Key), typeof(double));
            }

            // build rows.
            foreach (var alt in _alternatives)
            {
                var newRow = result.NewRow();
                newRow[ColumnHelper.GetAlternativeColumnName()] = alt.Key;

                foreach (var item in alt.Value)
                {
                    newRow[ColumnHelper.GetStakeholderColumnName(item.StakeholderId)] = item.Topsis;
                }

                result.Rows.Add(newRow);
            }

            return result;
        }
    }
}
