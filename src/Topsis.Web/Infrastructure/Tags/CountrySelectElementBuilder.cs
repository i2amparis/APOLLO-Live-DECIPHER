using Topsis.Domain;

namespace Topsis.Web.Infrastructure.Tags
{
    public class CountrySelectElementBuilder : EntitySelectElementBuilder<Country>
    {
        protected override object GetValue(Country instance)
        {
            return instance.Id;
        }

        protected override string GetDisplayValue(Country instance)
        {
            return instance.Title;
        }
    }

    public class JobCategorySelectElementBuilder : EntitySelectElementBuilder<JobCategory>
    {
        protected override object GetValue(JobCategory instance)
        {
            return instance.Id;
        }

        protected override string GetDisplayValue(JobCategory instance)
        {
            return instance.Title;
        }
    }
}