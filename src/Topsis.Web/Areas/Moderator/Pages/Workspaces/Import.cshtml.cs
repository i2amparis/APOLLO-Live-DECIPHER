using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using Topsis.Adapters.Import;
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
            string url = null;
            string error = null;

            if (file != null)
            {
                try
                {
                    var cmd = new ImportWorkspace.Command(file);
                    var workspace = await bus.SendAsync(cmd);
                    url = Url.Page(pageName: "/Workspaces/Edit", values: new { id = workspace.Id.Hash() });
                }
                catch (ImportException ie)
                {
                    error = ie.Message;
                }
                catch (Exception ex)
                {
                    error = ex.InnerException?.Message ?? ex.Message;
                }

                return new JsonResult(new { redirect = url, error });
            }

            return new OkResult();
        }
    }
}
