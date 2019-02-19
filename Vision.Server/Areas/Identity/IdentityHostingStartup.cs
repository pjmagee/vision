using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vision.Server.Areas.Identity.Data;

// [assembly: HostingStartup(typeof(Vision.Server.Areas.Identity.IdentityHostingStartup))]
namespace Vision.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => 
            {
                services.AddDbContext<IdentityDataContext>(options => options.UseSqlServer(context.Configuration.GetConnectionString("IdentityDataContextConnection")));
                services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<IdentityDataContext>();
            });
        }
    }
}