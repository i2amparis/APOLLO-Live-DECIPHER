namespace Topsis.Domain
{
    public class JobCategory : EntityWithTitle
    {
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
                new JobCategory(1, "Academia/Researcher"),
                new JobCategory(2, "Private Sector/Industry"),
                new JobCategory(3, "International Institution"),
                new JobCategory(4, "National Government"),
                new JobCategory(5, "Regional/Local Government"),
                new JobCategory(6, "Policymaker"),
                new JobCategory(7, "NGO"),
                new JobCategory(8, "Press"),
                new JobCategory(9, "Civil Society"),

                new JobCategory(100, "Other"),
            };
        }
    }
}
