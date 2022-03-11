using System.Text.RegularExpressions;

namespace Topsis.Adapters.Import
{
    public class SurveyColumnCriterionWeight : SurveyColumn
    {
        // https://regex101.com/r/VUEcUG/1
        private const string RegexGroupNumber = "number";
        private const string RegexGroupCriterion = "criterion";
        private static string RegexPattern = $@"^((\s*)(((?i)criteria)(\s*)((?i)weights))(\s*)(\[(?<{RegexGroupNumber}>\d)\.(\s*)(?<{RegexGroupCriterion}>(.*))\]))$";
        private static Regex Regex = new Regex(RegexPattern);

        public SurveyColumnCriterionWeight(int columnIndex, string rawTitle) : base(SurveyHeaderType.Category, columnIndex, rawTitle)
        {
            var match = Regex.Match(rawTitle);
            if (match.Success == false)
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidCriterionWeight, $"Could not parse criteria weights in '{rawTitle}'.");
            }

            Number = GetNumber(rawTitle, match);
            CriterionTitle = GetCriterion(rawTitle, match);
        }

        public int Number { get; }
        public string CriterionTitle { get; }

        private static int GetNumber(string rawTitle, Match match)
        {
            var number = match.Groups[RegexGroupNumber].Value;
            if (int.TryParse(number, out var criterionNumber) == false)
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidCriterionWeight, $"Cannot find number in criterion weight '{rawTitle}'.");
            }

            return criterionNumber;
        }

        private static string GetCriterion(string rawTitle, Match match)
        {
            var title = match.Groups[RegexGroupCriterion].Value;
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidCriterionWeight, $"Cannot find criterion in criterion weight '{rawTitle}'.");
            }

            return title;
        }
    }
}
