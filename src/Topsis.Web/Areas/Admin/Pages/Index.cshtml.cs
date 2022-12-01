using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public string Term { get; private set; }
        public PaginatedList<ApplicationUser> Data { get; private set; }

        public async Task OnGet([FromQuery] string term,
            [FromQuery(Name = "p")] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            Term = term;
            Data = await _reports.GetUsersAsync(term, page, pageSize);
        }
    }
}
