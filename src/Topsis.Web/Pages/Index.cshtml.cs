using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Database;
using Topsis.Domain.Contracts;

namespace Topsis.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IList<StakeholderWorkspaceDto> Workspaces { get; private set; }

        public async Task OnGetAsync([FromServices] IReportService reports,
            [FromServices] IUserContext userContext)
        {
            _logger.LogInformation("Fetching workspaces");
            Workspaces = await reports.GetStakeholderWorkspacesAsync(userContext, true);
        }
    }
}
