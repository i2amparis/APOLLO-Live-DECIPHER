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
    }
}
