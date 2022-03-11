using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Topsis.Application;
using Topsis.Application.Features;
using Topsis.Web.Pages;

namespace Topsis.Web.Areas.Moderator.Pages.Workspaces
{
    public class CreateModel : PageModel
    {
        private readonly IMessageBus _bus;

        [BindProperty]
        public CreateWorkspace.Command Data { get; set; }

        public CreateModel(IMessageBus bus)
        {
            _bus = bus;
        }

        public async Task<ActionResult> OnPostAsync()
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var id = await _bus.SendAsync(Data);
            return RedirectToPage("Edit", new { id });
        }
    }
}
