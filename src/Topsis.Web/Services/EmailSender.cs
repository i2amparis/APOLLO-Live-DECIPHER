using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using Topsis.Application;
using Topsis.Application.Features.SendEmail;

namespace Topsis.Web.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IMessageBus _bus;

        public EmailSender(IMessageBus bus)
        {
            _bus = bus;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await _bus.Send(new SendEmailCommand(email, subject, message));
        }
    }
}
