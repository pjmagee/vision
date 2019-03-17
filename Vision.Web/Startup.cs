using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vision.Web.Core;
using Vision.Web.Components;
using Microsoft.Extensions.Hosting;
using Vision.Web.Hubs;
using NSwag;

namespace Vision.Web
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) => this.configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();

            services
                .AddDbContext<VisionDbContext>(options => options
                    .UseLazyLoadingProxies(useLazyLoadingProxies: true)                    
                    .UseSqlServer(configuration["ConnectionStrings:Home"])
                    .ConfigureWarnings(warnings =>
                            warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)),
                            ServiceLifetime.Transient,
                            ServiceLifetime.Transient);

            services.AddSignalR();
            services.AddRazorComponents();
            services.AddMvc().AddNewtonsoftJson();                    

            RegisterVersionControlServices(services);
            RegisterAssetServices(services);
            RegisterDependencyServices(services);
            RegisterCiCdServices(services);
            RegisterRazorComponentServices(services);

            services.AddScoped<FakeDataGenerator>();
            services.AddScoped<IMetricService, MetricService>();

            RegisterRefreshServices(services);

            services.AddCors(options => options.AddPolicy("CorsPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()));

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

        private static void RegisterRefreshServices(IServiceCollection services)
        {
            services.AddScoped<ISystemTaskService, SystemTaskService>();            
            services.AddScoped<IRefreshService, RefreshService>();
            services.AddHostedService<BackgroundSystemRefreshMonitor>();
        }

        private static void RegisterRazorComponentServices(IServiceCollection services)
        {
            services.AddScoped<SvgService>();
            services.AddScoped<NavigationService>();
        }

        private static void RegisterCiCdServices(IServiceCollection services)
        {
            services.AddScoped<ICiCdService, CiCdService>();
            services.AddScoped<TeamCityProvider>();
            services.AddScoped<JenkinsProvider>();
            services.AddScoped<ICICDProvider, AggregateCICDProvider>();
        }

        private static void RegisterDependencyServices(IServiceCollection services)
        {
            services.AddScoped<IRegistryService, RegistryService>();
            services.AddScoped<IDependencyVersionService, DependencyVersionService>();
            services.AddScoped<IDependencyService, DependencyService>();

            services.AddScoped<NuGetVersionProvider>();
            services.AddScoped<DockerVersionProvider>();
            services.AddScoped<NpmVersionProvider>();
            services.AddScoped<IVersionProvider, AggregateVersionProvider>();
        }
        
        private static void RegisterAssetServices(IServiceCollection services)
        {
            services.AddScoped<IFrameworkService, FrameworkService>();
            services.AddScoped<IAssetService, AssetService>();
            services.AddScoped<IAssetDependencyService, AssetDependencyService>();

            services.AddScoped<NpmAssetExtractor>();
            services.AddScoped<NuGetAssetExtractor>();
            services.AddScoped<DockerAssetExtractor>();            
            services.AddScoped<IAssetExtractor, AggregateAssetExtractor>();
        }

        private static void RegisterVersionControlServices(IServiceCollection services)
        {
            services.AddScoped<IVersionControlService, VersionControlService>();
            services.AddScoped<IRepositoryService, RepositoryService>();
            services.AddScoped<IRepositoryMatcher, RepositoryMatcher>();

            services.AddScoped<BitBucketProvider>();
            services.AddScoped<GitlabProvider>();
            services.AddScoped<IVersionControlProvider, AggregateVersionControlProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseSignalR(builder => builder.MapHub<NotificationHub>("/notifications"));

            app.UseRouting(routes =>
            {
                routes.MapRazorPages();
                routes.MapComponentHub<App>("app");
            });
        }
    }
}
