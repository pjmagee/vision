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
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                    .UseSqlServer(configuration["ConnectionStrings:Default"])
                    .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)), ServiceLifetime.Transient);

            services.AddMvc(options => 
            {
                options.RespectBrowserAcceptHeader = false; // Only serve application/json

            }).AddNewtonsoftJson().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            services.AddScoped<BitBucketService>();
            services.AddScoped<GitlabService>();
            services.AddScoped<NpmExtractionService>();
            services.AddScoped<NuGetPackageExtractionService>();

            services.AddScoped<IBuildService, FakeBuildService>();
            services.AddScoped<IRefreshService, RefreshService>();
            services.AddScoped<IGitService, AggregateGitService>();
            services.AddScoped<IVersionService, AggregateVersionService>();
            services.AddScoped<IExtractionService, AggregateExtractionService>();

            services.AddHostedService<RefreshHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.AddScoped(provider => new HttpClient { BaseAddress = new Uri(provider.GetRequiredService<IUriHelper>().GetBaseUri()) });

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });

            services.AddSwaggerDocument(config => 
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Reed Business Information Vision API";
                    document.Info.Description = "Vision API for reporting and insights on organisation assets and dependencies";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.SwaggerContact
                    {
                        Name = "Patrick Magee",
                        Email = "patrick.magee@reedbusiness.com",
                        Url = "https://github.com/pjmagee"
                    };
                    document.Info.License = new NSwag.SwaggerLicense
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

            app.UseMvc();
            app.UseStaticFiles();
             // app.UseAuthentication();

            app.UseRazorComponents<App.Startup>();
        }
    }
}
