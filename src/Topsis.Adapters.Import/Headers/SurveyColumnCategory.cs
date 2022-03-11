using System.Text.RegularExpressions;

namespace Topsis.Adapters.Import
{
    public class SurveyColumnCategory : SurveyColumn
    {
        // https://regex101.com/r/ZZbi70/1
        private const string RegexGroupCategory = "category";
        private static string RegexPattern = $@"^((\s*)((?i)category)(\s*)(\[(?<{RegexGroupCategory}>(.*))\]))$";
        private static Regex Regex = new Regex(RegexPattern);

        public SurveyColumnCategory(int columnIndex, string rawTitle) : base(SurveyHeaderType.Category, columnIndex, rawTitle)
        {
            var match = Regex.Match(rawTitle);
            if (match.Success == false)
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidCategory, $"Could not parse category in '{rawTitle}'.");
            }

            var category = match.Groups[RegexGroupCategory].Value;
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ImportException(Domain.Common.DomainErrors.WorkspaceImport_InvalidCategory, $"Could not find category in '{rawTitle}'.");
            }

            CategoryName = category;
        }

        public string CategoryName { get; }
    }
}
