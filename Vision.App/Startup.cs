using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Vision.App
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(NavigationHelper.NavMenuItems);
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");            
        }
    }

}
