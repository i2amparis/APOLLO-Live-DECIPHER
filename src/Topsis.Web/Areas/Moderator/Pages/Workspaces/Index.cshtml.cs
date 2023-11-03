using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Topsis.Application;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Features;
using Topsis.Domain;

namespace Topsis.Web.Areas.Moderator.Pages.Workspaces
{
    [Authorize(Policy = Startup.RequireModeratorPolicy)]
    public class IndexModel : PageModel
    {
        private readonly IMessageBus _bus;

        public IPaginatedList<Workspace> Data { get; private set; }

        public IndexModel(IMessageBus bus)
        {
            _bus = bus;
        }

        public async Task OnGetAsync(int? pageNumber)
        {
            try
            {
                var response = await _bus.SendAsync(new GetWorkspace.ByPage.Request(pageNumber ?? 1));
                Data = response.Result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var command = new EditWorkspace.DeleteCommand { WorkspaceId = id };
            await _bus.SendAsync(command);
            return new OkObjectResult(new { success = true });
        }
    }
}
