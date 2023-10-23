using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain.Contracts;

namespace Topsis.Web.Areas.Admin.Pages
{
    public class UserModel : PageModel
    {
        public string Email { get; private set; }
        public IList<string> Roles { get; private set; }
        public string ErrorMessage { get; private set; }

        public async Task OnGet([FromServices] UserManager<ApplicationUser> users,
            [FromQuery] string email,
            [FromQuery] string error = null)
        {
            var user = await users.FindByEmailAsync(email);
            Email = user.Email;
            Roles = await users.GetRolesAsync(user);
            ErrorMessage = error;
        }

        public async Task<IActionResult> OnPostToggleRole([FromServices] UserManager<ApplicationUser> users,
            string role,
            string email, 
            bool add)
        {
            var user = await users.FindByEmailAsync(email);
            if (add)
            {
                await users.AddToRoleAsync(user, role);
            }
            else
            {
                if (role == RoleNames.Admin)
                {
                    // check if this is the last admin.
                    var adminUsers = await users.GetUsersInRoleAsync(role);
                    if (adminUsers.Count < 2)
                    {
                        return RedirectToPage("User",
                            new
                            {
                                email,
                                ts = DateTime.UtcNow.Ticks,
                                error = "There is only one admin, you cannot remove admin role."
                            });
                    }
                }

                await users.RemoveFromRoleAsync(user, role);
            }

            return RedirectToPage("User", new { email, ts = DateTime.UtcNow.Ticks });
        }
    }
}
