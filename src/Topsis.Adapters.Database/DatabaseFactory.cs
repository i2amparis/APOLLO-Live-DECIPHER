using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Topsis.Adapters.Database
{
    public class DatabaseFactory : IDesignTimeDbContextFactory<WorkspaceDbContext>
    {
        public WorkspaceDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WorkspaceDbContext>();
            optionsBuilder.AddMariaDbOptions("Server=127.0.0.1; port=3306; uid=root; pwd=password; database=topsis-test;");

            return new WorkspaceDbContext(optionsBuilder.Options, null);
        }
    }
}
