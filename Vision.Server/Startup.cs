﻿using System;
using System.Linq;
using NSwag;
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
using System.Net.Mime;
using Vision.Server.Hubs;

namespace Vision.Server
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) => this.configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorComponents<App.Startup>();


            services.AddConnections();

            services.AddSignalR().AddNewtonsoftJsonProtocol().AddHubOptions<NotificationHub>(o => 
            {
                o.EnableDetailedErrors = true;                
            });

            services.AddDbContext<VisionDbContext>(options => 
                options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(configuration["ConnectionStrings:Home"])
                    .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)), ServiceLifetime.Transient, ServiceLifetime.Transient);
            
            services.AddMvc().AddNewtonsoftJson().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

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
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });

            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowAnyOrigin()
                       .AllowCredentials();
            }));

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

            app.UseMvc();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseSignalR(route =>
            {
                route.MapHub<NotificationHub>("/notificationhub");
            });

            app.UseRazorComponents<App.Startup>();
        }
    }
}
