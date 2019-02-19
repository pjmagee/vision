using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vision.Core;

namespace Vision.Server
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorComponents<App.Startup>();

            // services.AddAutoMapper();

            services.AddDbContext<VisionDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(configuration["ConnectionStrings:Default"]));

            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped<IDependencyRepository, DependencyRepository>();
            services.AddScoped<IDependencyVersionRepository, DependencyVersionRepository>();
            services.AddScoped<IAssetDependencyRepository, AssetDependencyRepository>();
            services.AddScoped<IRegistryRepository, RegistryRepository>();
            services.AddScoped<IGitSourceRepository, GitSourceRepository>();
            services.AddScoped<IGitRepositoryRepository, GitRepositoryRepository>();
            

            services.AddMvc().AddNewtonsoftJson().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddScoped(provider => new HttpClient { BaseAddress = new Uri(provider.GetRequiredService<IUriHelper>().GetBaseUri()) });

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvc();
            app.UseStaticFiles();
             // app.UseAuthentication();

            app.UseRazorComponents<App.Startup>();
        }
    }
}
