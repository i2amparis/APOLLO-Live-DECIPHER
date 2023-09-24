using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Topsis.Application;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Application.Contracts.Security;
using Topsis.Application.Features;
using Topsis.Domain;
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
        public WorkspaceSettings WorkspaceSettings { get; private set; }

        private string GetReturnUrl(string returnUrl)
        {
            return returnUrl ?? Url.Content("~/Guest");
        }

        public RegisterModel(IMessageBus bus, SignInManager<ApplicationUser> signInManager)
        {
            _bus = bus;
            _signInManager = signInManager;
        }

        public async Task OnGet([FromServices] IMessageBus bus,
            string returnUrl = null, 
            string cid = "300", 
            int jid = 1)
        {
            WorkspaceSettings = new WorkspaceSettings();

            if (string.IsNullOrEmpty(returnUrl) == false)
            {
                var query = returnUrl.Split('?').LastOrDefault();
                var collection = HttpUtility.ParseQueryString(query);
                var id = collection.GetValues("id").FirstOrDefault();
                if (string.IsNullOrEmpty(id) == false)
                {
                    var response = await _bus.SendAsync(new GetWorkspace.ById.Request(id));
                    WorkspaceSettings = response.Result.GetSettings();
                }
            }

            ReturnUrl = GetReturnUrl(returnUrl);

            Data = new CreateStakeholder.Command
            {
                Country = new Domain.Country { Id = cid },
                JobCategory = new Domain.JobCategory { Id = jid },
                Gender = Domain.Contracts.Gender.NotAnswered
            };
        }

        public async Task<ActionResult> OnPostAsync([FromServices] IRecaptchaService recaptcha,
            [FromForm(Name = "g-recaptcha-response")] string recaptchaToken,
            string returnUrl = null)
        {
            ReturnUrl = GetReturnUrl(returnUrl);

            var valid = ModelState.IsValid;
            if (valid)
            {
                var errorMessage = await recaptcha.ValidateAsync(recaptchaToken, RecaptchaActions.GUEST_REGISTER);
                if (string.IsNullOrWhiteSpace(errorMessage) == false)
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    valid = false;
                }
            }

            if (valid)
            {
                var user = await _bus.SendAsync(Data);
                await _signInManager.SignInAsync(user, true);
                return Redirect(returnUrl);
            }
               
            return Page();
        }
    }
}
