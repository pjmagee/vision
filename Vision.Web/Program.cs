using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vision.Web.Core;

namespace Vision.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                using (var scope = host.Services.CreateScope())
                {
                    //FakeDataGenerator fakeDataGenerator = scope.ServiceProvider.GetRequiredService<FakeDataGenerator>();
                    //await fakeDataGenerator.SeedAsync();

                    //var context = scope.ServiceProvider.GetRequiredService<VisionDbContext>();
                    //await context.Database.EnsureDeletedAsync();
                    //await context.Database.EnsureCreatedAsync();
                }

                await host.RunAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}
