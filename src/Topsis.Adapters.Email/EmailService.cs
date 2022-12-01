using Microsoft.Extensions.Options;
using System.Net.Mail;
using System;
using System.Threading.Tasks;
using Topsis.Application.Features.SendEmail;
using Topsis.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Topsis.Adapters.Email
{

    public class EmailService : IEmailService
    {
        //
        // test smtp server: https://www.wpoven.com/tools/free-smtp-server-for-testing
        //

        private readonly ILogger<EmailService> _logger;
        private SmtpSettings _settings;

        public EmailService(IOptions<SmtpSettings> mailSettings, ILogger<EmailService> logger)
        {
            _settings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendWithSmtpAsync(SendEmailCommand request)
        {
            try
            {
                _logger.LogInformation($"Starting sending to 'email:{request.ToEmail}'.");

                // use the Gmail SMTP Host
                var client = new SmtpClient(_settings.Host)
                {
                    EnableSsl = _settings.EnableSsl,
                    Port = _settings.Port,
                    Credentials = new System.Net.NetworkCredential(_settings.SenderEmail, _settings.SenderPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 20000
                };

                using (var newMail = new MailMessage()
                {
                    From = new MailAddress(_settings.SenderEmail, _settings.SenderDisplayName),
                    Subject = request.Subject,
                    IsBodyHtml = true,
                    Body = request.Body
                })
                { 
                    newMail.To.Add(request.ToEmail);
                    await client.SendMailAsync(newMail);
                }

                _logger.LogInformation($"Finished sending to 'email:{request.ToEmail}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, _settings.SenderEmail, request.ToEmail);
            }
        }
    }
}
