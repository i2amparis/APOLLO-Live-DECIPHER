using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Topsis.Application.Interfaces;

namespace Topsis.Adapters.Email
{
    public static class Startup
    {
        public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
