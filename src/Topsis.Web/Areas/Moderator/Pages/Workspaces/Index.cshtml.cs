using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
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
                var response = await _bus.Send(new GetWorkspace.ByPage.Request(pageNumber ?? 1));
                Data = response.Result;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
    }
}
