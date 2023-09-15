namespace Topsis.Domain
{
    public class JobCategory : EntityWithTitle
    {
        public const int OtherJobCategoryId = 100;

        public JobCategory() : base()
        {
        }

        public JobCategory(int id, string title): base(id, title)
        {
        }

        public static JobCategory[] AllJobCategories()
        {
            
            return new[]
            {
                new JobCategory(1, "Academia/Research"),
                new JobCategory(2, "Private Sector/Industry"),
                new JobCategory(3, "International Institution"),
                new JobCategory(4, "National Government"),
                new JobCategory(5, "Regional/Local Government"),
                new JobCategory(6, "Policymaker"),
                new JobCategory(7, "NGO"),
                new JobCategory(8, "Press"),
                new JobCategory(9, "Civil Society"),
                new JobCategory(10, "Consultant"),
                new JobCategory(11, "Philanthropy"),
                new JobCategory(12, "Central Bank"),
                new JobCategory(13, "Diplomat"),
                new JobCategory(14, "Regional Energy Agency"),
                new JobCategory(15, "Think tank"),
                new JobCategory(16, "Responsible investment advisory firm (and actuary)"),
                new JobCategory(17, "Multiple affiliations; Gov, NGO, Ac & industry"),
                new JobCategory(18, "Citizen"),

                new JobCategory(OtherJobCategoryId, "Other"),
            };
        }
    }
}
