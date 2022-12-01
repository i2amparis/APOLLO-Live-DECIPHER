using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Topsis.Application.Interfaces;

namespace Topsis.Application.Features.SendEmail
{
    public class SendEmailCommand : IRequest<Unit>
    {
        public SendEmailCommand(string toEmail, string subject, string body)
        {
            ToEmail = toEmail;
            Subject = subject;
            Body = body;
        }

        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public override string ToString()
        {
            return $@"
To: {ToEmail}
Subject: {Subject}
Body: {Body}";
        }
    }

    public class SendEmailHandler : IRequestHandler<SendEmailCommand>
    {
        private readonly IEmailService _emailService;

        public SendEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<Unit> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            Debug.WriteLine("EMAIL: " + request);
            await _emailService.SendWithSmtpAsync(request);
            return Unit.Value;
        }
    }
}
