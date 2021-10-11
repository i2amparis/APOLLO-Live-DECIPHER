using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Topsis.Web.Areas.Identity.IdentityHostingStartup))]
namespace Topsis.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                
            });
        }
    }
}