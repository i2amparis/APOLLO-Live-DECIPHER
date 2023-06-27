using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Topsis.Web
{
    public class Program
    {
        public static string Version = "1.0.12";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
