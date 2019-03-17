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

            services.AddScoped<FakeDataGenerator>();

            RegisterVersionControlServices(services);
            RegisterAssetServices(services);
            RegisterDependencyServices(services);
            RegisterCiCdServices(services);
            RegisterRazorComponentServices(services);

            services.AddScoped<MetricItemsService>();

            RegisterSystemRefreshServices(services);

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

        private static void RegisterSystemRefreshServices(IServiceCollection services)
        {
            services.AddScoped<SystemTaskService>();
            services.AddHostedService<BackgroundSystemRefreshMonitor>();
            services.AddScoped<ISystemRefreshService, SystemRefreshService>();
        }

        private static void RegisterRazorComponentServices(IServiceCollection services)
        {
            services.AddScoped<SvgService>();
            services.AddScoped<NavigationService>();
        }

        private static void RegisterCiCdServices(IServiceCollection services)
        {
            services.AddScoped<CiCdsService>();
            services.AddScoped<TeamCityBuildsService>();
            services.AddScoped<JenkinsBuildsService>();
            services.AddScoped<ICICDBuildsService, AggregateCICDBuildsService>();
        }

        private static void RegisterDependencyServices(IServiceCollection services)
        {
            services.AddScoped<RegistriesService>();
            services.AddScoped<DependencyVersionService>();
            services.AddScoped<DependenciesService>();

            services.AddScoped<NuGetVersionService>();
            services.AddScoped<DockerVersionService>();
            services.AddScoped<NpmVersionService>();
            services.AddScoped<IVersionService, AggregateVersionService>();
        }
        
        private static void RegisterAssetServices(IServiceCollection services)
        {
            services.AddScoped<FrameworksService>();
            services.AddScoped<AssetsService>();
            services.AddScoped<RepositoryAssetsService>();
            services.AddScoped<AssetDependenciesService>();

            services.AddScoped<NpmAssetExtractor>();
            services.AddScoped<NuGetAssetExtractor>();
            services.AddScoped<DockerAssetExtractor>();
            services.AddScoped<IAssetExtractor, AggregateAssetExtractor>();
        }

        private static void RegisterVersionControlServices(IServiceCollection services)
        {
            services.AddScoped<VersionControlsService>();
            services.AddScoped<RepositoriesService>();

            services.AddScoped<IRepositoryMatcher, RepositoryMatcher>();
            services.AddScoped<BitBucketService>();
            services.AddScoped<GitlabService>();
            services.AddScoped<IVersionControlService, AggregateVersionControlService>();
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
