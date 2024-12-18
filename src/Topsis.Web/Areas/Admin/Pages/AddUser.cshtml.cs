using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Identity;
using static Google.Cloud.RecaptchaEnterprise.V1.AccountVerificationInfo.Types;

namespace Topsis.Web.Areas.Admin.Pages
{
    public class AddUserModel : PageModel
    {
        public string Email { get; private set; }
        public string ErrorMessage { get; private set; }

        public void OnGet([FromQuery] string email,
            [FromQuery] string error = null)
        {
            ErrorMessage = error;
        }

        public async Task<IActionResult> OnPost([FromServices] UserManager<ApplicationUser> users, 
            [FromForm] string email,
            [FromForm] string password,
            [FromForm] string password2)
        {
            try
            {
                var user = await users.FindByEmailAsync(email);
                if (user != null)
                {
                    return Error(email, "User already exists.");
                }

                if (string.Equals(password, password2) == false)
                {
                    return Error(email, "Passwords do not match.");
                }

                var newUser = new ApplicationUser { UserName = email, Email = email };
                var result = await users.CreateAsync(newUser, password);
                if (!result.Succeeded)
                {
                    return Error(email, string.Join(" ", result.Errors.Select(e => e.Description)));
                }
            }
            catch (System.Exception ex)
            {
                return Error(email, "Unknown error");
            }

            return RedirectToPage("Index");
        }

        private IActionResult Error(string email, string error)
        {
            return RedirectToPage("AddUser",
                    new
                    {
                        email,
                        ts = DateTime.UtcNow.Ticks,
                        error = error
                    });
        }
    } 
}
