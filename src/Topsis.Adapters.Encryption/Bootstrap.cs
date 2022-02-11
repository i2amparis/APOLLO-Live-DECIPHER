using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Topsis.Application.Contracts.Identity;

namespace Topsis.Adapters.Encryption
{
    public static class Bootstrap
    {
        public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            var config = new GoogleSettings();
            configuration.GetSection(nameof(GoogleSettings)).Bind(config);

            // Antiforgery tokens require data protection.
            if (config.IsValid())
            {
                services.AddDataProtection()
                    // Store keys in Cloud Storage so that multiple instances
                    // of the web application see the same keys.
                    .PersistKeysToGoogleCloudStorage(config.DataProtection.Bucket,
                        config.DataProtection.Object)
                    // Protect the keys with Google KMS for encryption and fine-
                    // grained access control.
                    .ProtectKeysWithGoogleKms(config.DataProtection.KmsKeyName);

            }
            else
            {
                services.AddDataProtection();
            }


            return services;
        }
    }
}
