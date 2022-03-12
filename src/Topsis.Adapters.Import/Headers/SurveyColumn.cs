namespace Topsis.Adapters.Import
{
    public enum SurveyHeaderType
    {
        StakeholderIdentifier = 0,
        Category = 1,
        CriterionAlternative = 2,
        CriterionWeight = 3,
        StakeholderWeight = 4
    }

    public abstract class SurveyColumn
    {
        private const string COLUMN_STAKEHOLDERID = "ID";
        private const string COLUMN_CATEGORY = "CATEGORY";
        private const string COLUMN_CRITERIA_WEIGHTS = "CRITERIA WEIGHTS";
        private const string COLUMN_STAKEHOLDER_WEIGHTS = "STAKEHOLDER WEIGHTS";


        public SurveyColumn(SurveyHeaderType type, int columnIndex, string rawTitle)
        {
            RawTitle = rawTitle?.Trim();
            Type = type;
            ColumnIndex = columnIndex;
        }

        public string RawTitle { get; }
        public SurveyHeaderType Type { get; }
        public int ColumnIndex { get; }

        public static SurveyColumn From(int columnIndex, string rawTitle)
        {
            rawTitle = rawTitle?.Trim() ?? string.Empty;
            var rawTitleUpper = rawTitle.ToUpper();

            if (string.IsNullOrEmpty(rawTitleUpper) || string.Equals(COLUMN_STAKEHOLDERID, rawTitleUpper))
            {
                return new SurveyColumnStakeholderId(columnIndex, rawTitle);
            }

            if (rawTitleUpper.StartsWith(COLUMN_CATEGORY))
            {
                return new SurveyColumnCategory(columnIndex, rawTitle);
            }

            if (rawTitleUpper.StartsWith(COLUMN_STAKEHOLDER_WEIGHTS))
            {
                return new SurveyColumnStakeholderWeight(columnIndex, rawTitle);
            }

            if (rawTitleUpper.StartsWith(COLUMN_CRITERIA_WEIGHTS))
            {
                return new SurveyColumnCriterionWeight(columnIndex, rawTitle);
            }

            return new SurveyColumnCriterion(columnIndex, rawTitle);
        }
    }
}
