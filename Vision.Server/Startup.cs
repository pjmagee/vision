using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using NSwag;
using Vision.Core;
using Vision.Core.Services.Builds;
using Vision.Server.Services;

namespace Vision.Server
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorComponents<App.Startup>();

            services.AddDbContext<VisionDbContext>(options => 
                options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(configuration["ConnectionStrings:Home"])
                    .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)), ServiceLifetime.Transient, ServiceLifetime.Transient);

            // services.AddResponseCaching();

            services.AddMvc(options => 
            {
                options.RespectBrowserAcceptHeader = false; // Only serve application/json

            }).AddNewtonsoftJson().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddScoped<BitBuckerChecker>();
            services.AddScoped<GitlabChecker>();
            services.AddScoped<NodePackagesExtractor>();
            services.AddScoped<NuGetPackageExtractor>();

            services.AddScoped<ICiCdChecker, FakeBuildsChecker>();
            services.AddScoped<IRefreshService, RefreshService>();
            services.AddScoped<IVcsChecker, AggregateVcsChecker>();
            services.AddScoped<IVersionChecker, AggregateVersionChecker>();
            services.AddScoped<IDependencyExtractor, AggregateExtractor>();

            services.AddHostedService<RefreshHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.AddScoped(provider => new System.Net.Http.HttpClient { BaseAddress = new Uri(provider.GetRequiredService<IUriHelper>().GetBaseUri()) });

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    System.Net.Mime.MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });

            services.AddSwaggerDocument(config => 
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Vision API";
                    document.Info.Description = "Vision API for asset and dependency reporting for the organisation";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new SwaggerContact
                    {
                        Name = "Patrick Magee",
                        Email = "patrick.magee@reedbusiness.com",
                        Url = "https://github.com/pjmagee"
                    };
                    document.Info.License = new SwaggerLicense
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    };
                };

            });
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUi3();

            //app.UseResponseCaching();

            //app.Use(async (context, next) =>
            //{
            //    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue() { Public = true, MaxAge = TimeSpan.FromSeconds(10) };
            //    context.Response.Headers[HeaderNames.Vary] = new string[] { "Accept-Encoding" };
            //    await next();
            //});

            app.UseMvc();
            app.UseStaticFiles();
             // app.UseAuthentication();

            app.UseRazorComponents<App.Startup>();
        }
    }
}
