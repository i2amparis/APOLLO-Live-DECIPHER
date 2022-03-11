using System.Text.RegularExpressions;

namespace Topsis.Adapters.Import
{
    public class SurveyColumnCriterion : SurveyColumn
    {
        // https://regex101.com/r/VYEInw/1
        private const string RegexGroupNumber = "number";
        private const string RegexGroupCriterion = "criterion";
        private const string RegexGroupAlternative = "alternative";
        private static string RegexPattern = $@"^((?<{RegexGroupNumber}>\d)\.(?<{RegexGroupCriterion}>(.*))(\[(?<{RegexGroupAlternative}>(.*))\]))$";
        private static Regex Regex = new Regex(RegexPattern);

        public SurveyColumnCriterion(int columnIndex, string rawTitle) : base(SurveyHeaderType.CriterionAlternative, columnIndex, rawTitle)
        {
            var match = Regex.Match(rawTitle);
            if (match.Success == false)
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidCriterion, $"Criterion '{rawTitle}' could not be parsed.");
            }

            Number = GetNumber(rawTitle, match);
            CriterionTitle = GetCriterion(rawTitle, match);
            AlternativeTitle = GetAlternative(rawTitle, match);
        }

        private static int GetNumber(string rawTitle, Match match)
        {
            var number = match.Groups[RegexGroupNumber].Value;
            if (int.TryParse(number, out var criterionNumber) == false)
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidCriterion, $"Cannot find number criterion in '{rawTitle}'.");
            }

            return criterionNumber;
        }

        private static string GetCriterion(string rawTitle, Match match)
        {
            var title = match.Groups[RegexGroupCriterion].Value.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidCriterion, $"Cannot find criterion title in '{rawTitle}' could not find criterion question.");
            }

            return title;
        }

        private static string GetAlternative(string rawTitle, Match match)
        {
            var title = match.Groups[RegexGroupAlternative].Value.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidCriterion, $"Criterion '{rawTitle}' could not find alternative.");
            }

            return title;
        }

        public int Number { get; }
        public string CriterionTitle { get; }
        public string AlternativeTitle { get; }
        public string AlternativeKey => AlternativeTitle.Replace(" ", string.Empty).ToLower();
    }
}
