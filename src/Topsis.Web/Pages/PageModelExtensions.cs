using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Topsis.Domain;

namespace Topsis.Web.Pages
{
    public static class PageModelExtensions
    {
        public static ActionResult RedirectToPageJson<TPage>(this TPage controller, string pageName, object values = null)
            where TPage : PageModel =>
            controller.JsonNet(new
            {
                redirect = controller.Url.Page(pageName, values)
            }
            );

        public static ContentResult JsonNet(this PageModel controller, object model)
        {
            var serialized = JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return new ContentResult
            {
                Content = serialized,
                ContentType = "application/json"
            };
        }

    }

    public static class StringExtensions
    {
        public static string ToBootstrap(this WorkspaceStatus status)
        {
            return status switch
            {
                WorkspaceStatus.AcceptingVotes => "active",
                _ => "",
            };
        }
    }
}
