using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;

namespace Vision.Web
{

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            
            services.AddMemoryCache();
            
            services.AddRazorPages();
            
            services.AddServerSideBlazor();
            
            services.AddVisionServices(Configuration);
            
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()));
            
            services.AddOpenApiDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Vision API";
                    document.Info.Description = "Vision API for asset and dependency reporting for the organisation";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "Patrick Magee",
                        Email = "patrick.magee@reedbusiness.com",
                        Url = "https://github.com/pjmagee"
                    };
                    document.Info.License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    };
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
