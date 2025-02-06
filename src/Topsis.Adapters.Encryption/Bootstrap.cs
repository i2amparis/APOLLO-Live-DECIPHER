using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Topsis.Adapters.Encryption.Services;
using Topsis.Application.Contracts.Database;
using Topsis.Application.Contracts.Identity;
using Topsis.Application.Contracts.Security;

namespace Topsis.Adapters.Encryption
{
    public static class Bootstrap
    {
        public static IServiceCollection AddRecaptcha(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RecaptchaSettings>(configuration.GetSection("RecaptchaSettings"));

            services.AddSingleton<IRecaptchaService, RecaptchaService>();
            return services;
        }

        public static IServiceCollection AddDataProtectionToDatabase(this IServiceCollection services)
        {
            services.AddDbContext<DataProtectionKeysContext>((serviceProvider, dbContextBuilder) =>
            {
                var service = serviceProvider.GetRequiredService<IDatabaseService>();
                DatabaseFactory.SetupDatabase(dbContextBuilder, service.GetMigrationConnectionString());
            });

            services.AddDataProtection()
                .PersistKeysToDbContext<DataProtectionKeysContext>();

            return services;
        }

        public static IServiceCollection AddGoogleDataProtection(this IServiceCollection services, IConfiguration configuration)
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

        public static IServiceCollection AddDataProtectionToFileSystem(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(GetKeyRingDirectoryInfo(configuration))
                .SetApplicationName("SharedCookieApp");

            return services;
        }

        private static DirectoryInfo GetKeyRingDirectoryInfo(IConfiguration configuration)
        {
            //string applicationBasePath = System.AppContext.BaseDirectory;
            //var directoryInfo = new DirectoryInfo(applicationBasePath);
            string keyRingPath = configuration.GetSection("AppKeys").GetValue<string>("keyRingPath");

            if (Directory.Exists(keyRingPath) == false)
            {
                return Directory.CreateDirectory(keyRingPath);
            }

            return new DirectoryInfo(keyRingPath);
        }
    }
}
