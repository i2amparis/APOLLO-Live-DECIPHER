using System;
using System.Data;

namespace Topsis.Adapters.Algorithm
{
    internal static class DataTableHelper
    {
        public static (double, double) GetColumnMinMax(this DataTable table, string column)
        {
            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (DataRow item in table.Rows)
            {
                var cellValue = item.Field<double>(column);
                min = Math.Min(min, cellValue);
                max = Math.Min(min, cellValue);
            }

            return (min, max);
        }
    }

    internal static class ColumnHelper
    {
        internal static string GetCriterionColumnName(int criterionId)
        {
            return $"c_{criterionId}";
        }

        internal static string GetStakeholderColumnName(string stakeholderId)
        {
            return $"s_{stakeholderId}";
        }

        internal static string GetAlternativeColumnName()
        {
            return "alternative";
        }

        internal static string GetAlternativeTitleColumnName()
        {
            return "alternative_title";
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

        internal static string GetStakeholderDistanceColumnName(string stakeholderId, bool isPositive)
        {
            var suffix = isPositive ? "positive" : "negative";
            return $"s_{stakeholderId}_{suffix}";
        }

        internal static string GetTopsisColumnName()
        {
            return "topsis";
        }
    }
}
