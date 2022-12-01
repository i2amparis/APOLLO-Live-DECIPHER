using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain.Contracts;

namespace Topsis.Web.Areas.Admin.Pages
{
    [Authorize(Policy = Startup.RequireAdminPolicy)]
    public class IndexModel : PageModel
    {
        private readonly IReportService _reports;

        public IndexModel(IReportService reports)
        {
            _reports = reports;
        }

        public IList<ApplicationUser> Users { get; private set; }

        public async Task OnGet()
        {
            Users = await _reports.GetUsersAsync();
        }
    }
}
