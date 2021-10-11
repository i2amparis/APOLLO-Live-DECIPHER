using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Topsis.Application.Features.SendEmail;
using Topsis.Application.Interfaces;

namespace Topsis.Adapters.Email
{

    public class EmailService : IEmailService
    {
        private SmtpSettings _settings;

        public EmailService(IOptions<SmtpSettings> mailSettings)
        {
            _settings = mailSettings.Value;
        }

        public Task ExecuteAsync(SendEmailCommand request)
        {
            // todo:..
            return Task.CompletedTask;
        }
    }
}
