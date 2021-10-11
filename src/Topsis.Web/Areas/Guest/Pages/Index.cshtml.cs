using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Domain.Contracts;

namespace Topsis.Web.Areas.Guest.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IReportService _reports;
        private readonly IUserContext _userContext;

        public IndexModel(IReportService reports, IUserContext userContext)
        {
            _reports = reports;
            _userContext = userContext;
        }

        public IList<StakeholderWorkspaceDto> Workspaces { get; private set; }

        public async Task OnGet()
        {
            Workspaces = await _reports.GetStakeholderWorkspacesAsync(_userContext);
        }
    }
}
