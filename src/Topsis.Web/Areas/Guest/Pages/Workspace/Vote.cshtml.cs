using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Topsis.Application;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Features;
using Topsis.Domain.Contracts;
using Topsis.Web.Pages;

namespace Topsis.Web.Areas.Guest.Pages.Workspace
{
    public class VoteModel : PageModel
    {
        // https://www.learnrazorpages.com/razor-pages/model-binding

        public StakeholderVoteViewModel ViewModel { get; set; }

        [BindProperty]
        public Vote.Command Data { get; set; }

        public async Task<IActionResult> OnGetAsync([FromServices] IUserContext userContext,
            [FromServices] IReportService reports,
            string id)
        {
            if (string.IsNullOrEmpty(userContext?.UserId) || string.IsNullOrEmpty(id))
            {
                return RedirectToPage("/Index");
            }

            ViewModel = await reports.GetStakeholderVoteViewModelAsync(userContext, id);

            if (ViewModel == null || ViewModel.WorkspaceStatus != Domain.WorkspaceStatus.AcceptingVotes)
            {
                return RedirectToPage("/Index");
            }

            Data = new Vote.Command { Id = id };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] IMessageBus bus)
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var result = await bus.Send(Data);
            return this.RedirectToPage("/Index");
        }
    }
}
