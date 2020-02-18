using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Vision.Core;

namespace Vision.Web.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureServices(services => services.AddVisionHostedBackgroundServices());
        }
    }
}
