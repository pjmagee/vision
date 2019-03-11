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
            
            services.AddScoped<BitBucketService>();
            services.AddScoped<GitlabService>();

            

            services.AddScoped<NPMAssetExtractor>();
            services.AddScoped<NuGetAssetExtractor>();
            services.AddScoped<DockerAssetExtractor>();
            services.AddScoped<NuGetVersionService>();
            services.AddScoped<DockerVersionService>();
            services.AddScoped<NPMVersionService>();
            
            services.AddScoped<TeamCityBuildsService>();
            services.AddScoped<JenkinsBuildsService>();
            services.AddScoped<RepositoryMatcher>();

            services.AddScoped<IRepositoryMatcher, RepositoryMatcher>();
            services.AddScoped<ISystemTaskService, SystemTaskService>();
            services.AddScoped<IVersionControlService, AggregateVersionControlService>();
            services.AddScoped<ICICDBuildsService, AggregateBuildsService>();
            services.AddScoped<IAssetExtractor, AggregateAssetExtractor>();
            services.AddScoped<IVersionService, AggregateVersionService>();

            services.AddScoped<AssetsService>();
            services.AddScoped<CiCdsService>();
            services.AddScoped<DependenciesService>();
            services.AddScoped<DependencyVersionsService>();
            services.AddScoped<TasksService>();
            services.AddScoped<DashboardService>();
            services.AddScoped<FrameworksService>();
            services.AddScoped<InsightsService>();
            services.AddScoped<RepositoriesService>();
            services.AddScoped<RegistriesService>();
            services.AddScoped<VersionControlsService>();
            
            services.AddScoped<SvgService>();
            services.AddScoped<NavigationService>();

            services.AddHostedService<RefreshHostedService>();

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowAnyOrigin();
            }));

            // services.AddScoped(provider => new HttpClient() { BaseAddress = new Uri(provider.GetRequiredService<IUriHelper>().GetBaseUri()) });

            //services.AddSwaggerDocument(config => 
            //{
            //    config.PostProcess = document =>
            //    {
            //        document.Info.Version = "v1";
            //        document.Info.Title = "Vision API";
            //        document.Info.Description = "Vision API for asset and dependency reporting for the organisation";
            //        document.Info.TermsOfService = "None";
            //        document.Info.Contact = new SwaggerContact
            //        {
            //            Name = "Patrick Magee",
            //            Email = "patrick.magee@reedbusiness.com",
            //            Url = "https://github.com/pjmagee"
            //        };
            //        document.Info.License = new SwaggerLicense
            //        {
            //            Name = "Use under LICX",
            //            Url = "https://example.com/license"
            //        };
            //    };
            //});
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseSignalR(builder => 
            {
                builder.MapHub<NotificationHub>("/notifications");
            });

            app.UseRouting(routes =>
            {
                routes.MapRazorPages();
                routes.MapComponentHub<App>("app");
            });
        }
    }
}
