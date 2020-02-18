using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vision.Core;
using Vision.Web.Blazor.Services;

namespace Vision.Web.Blazor
{
    public static class BlazorServices
    {
        public static IServiceCollection RegisterVisionUIServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<SvgService>()
                .AddSingleton<IPager, Pager>()
                .AddScoped<NavigationService>();
        }
    }


}
