using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Topsis.Application;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Application.Features;
using Topsis.Web.Pages;

namespace Topsis.Web.Areas.Guest.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IMessageBus _bus;
        private readonly SignInManager<ApplicationUser> _signInManager;

        [BindProperty]
        public CreateStakeholder.Command Data { get; set; }

        public string ReturnUrl { get; set; }

        private string GetReturnUrl(string returnUrl)
        {
            return returnUrl ?? Url.Content("~/Guest");
        }

        public RegisterModel(IMessageBus bus, SignInManager<ApplicationUser> signInManager)
        {
            _bus = bus;
            _signInManager = signInManager;
        }

        public void OnGet(string returnUrl = null, string cid = "300", int jid = 1)
        {
            ReturnUrl = GetReturnUrl(returnUrl);

            Data = new CreateStakeholder.Command
            {
                Country = new Domain.Country { Id = cid },
                JobCategory = new Domain.JobCategory { Id = jid }
            };
        }

        public async Task<ActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = GetReturnUrl(returnUrl);

            var user = await _bus.SendAsync(Data);
            await _signInManager.SignInAsync(user, true);
            return Redirect(returnUrl);
        }
    }
}
