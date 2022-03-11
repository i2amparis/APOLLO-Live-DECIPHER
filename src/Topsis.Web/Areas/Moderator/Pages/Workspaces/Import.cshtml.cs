using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Topsis.Application;
using Topsis.Application.Features;
using Topsis.Domain.Common;

namespace Topsis.Web.Areas.Moderator.Pages.Workspaces
{
    public class ImportModel : PageModel
    {
        public void OnGet()
        {
        }

        public const long MaxFileBytesLength = 10_000_000; // 10MB
        public double MaxFileMbLength => MaxFileBytesLength / 1000_000d;

        public async Task<IActionResult> OnPostAsync([FromServices] IMessageBus bus,
            IFormFile file)
        {
            if (file != null)
            {
                var cmd = new ImportWorkspace.Command(file);
                var workspace = await bus.SendAsync(cmd);
                var url = Url.Page(pageName: "/Workspaces/Edit", values: new { id = workspace.Id.Hash() });

                return new JsonResult(new { redirect = url });
            }

            return new OkResult();
        }
    }
}
