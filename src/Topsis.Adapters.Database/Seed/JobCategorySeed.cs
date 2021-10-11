using Topsis.Domain;

namespace Topsis.Adapters.Database.Seed
{
    public static class JobCategorySeed
    {
        public static JobCategory[] Data()
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