using System.Threading.Tasks;
using Topsis.Application.Features.SendEmail;

namespace Topsis.Application.Interfaces
{

    public interface IEmailService
    {
        Task SendWithSmtpAsync(SendEmailCommand request);
    }
}
