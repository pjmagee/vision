using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vision.Web.Core;

namespace Vision.Web
{
    public static class VisionServices
    {
        public static IServiceCollection AddVisionServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbContext<VisionDbContext>(options => options
                    .UseLazyLoadingProxies(useLazyLoadingProxies: true)
                    // .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseSqlServer(configuration["ConnectionStrings:Default"]), ServiceLifetime.Transient, ServiceLifetime.Transient);

            services
                .RegisterVersionControlServices()
                .RegisterAssetServices()
                .RegisterDependencyServices()
                .RegisterCiCdServices()
                .RegisterRazorComponentServices()
                .AddSingleton<IEncryptionService, EncryptionService>()
                .AddTransient<FakeDataGenerator>()
                .AddTransient<IMetricService, MetricService>();

            return services;
        }

        public static IServiceCollection RegisterRefreshServices(this IServiceCollection services) => services
            .AddScoped<ISystemTaskService, SystemTaskService>()
            .AddScoped<IRefreshService, RefreshService>()
            .AddHostedService<BackgroundSystemRefreshMonitor>();

        private static IServiceCollection RegisterRazorComponentServices(this IServiceCollection services) => services
            .AddSingleton<SvgService>()
            .AddScoped<NavigationService>();

        private static IServiceCollection RegisterCiCdServices(this IServiceCollection services) => services
            .AddTransient<ICiCdService, CiCdService>()
            .AddTransient<ICiCdProvider, TeamCityProvider>()
            .AddTransient<ICiCdProvider, JenkinsProvider>()
            .AddTransient<IAggregateCICDBuildsProvider, AggregateCICDBuildsProvider>();

        private static IServiceCollection RegisterDependencyServices(this IServiceCollection services) => services
            .AddTransient<IRegistryService, RegistryService>()
            .AddTransient<IDependencyVersionService, DependencyVersionService>()
            .AddTransient<IDependencyService, DependencyService>()
            .AddTransient<IDependencyVersionProvider, NuGetVersionProvider>()
            .AddTransient<IDependencyVersionProvider, DockerVersionProvider>()
            .AddTransient<IDependencyVersionProvider, NpmVersionProvider>()
            .AddTransient<IAggregateDependencyVersionProvider, AggregateDependencyVersionProvider>();

        private static IServiceCollection RegisterAssetServices(this IServiceCollection services) => services
            .AddTransient<IRuntimeService, RuntimeService>()
            .AddTransient<IAssetService, AssetService>()
            .AddTransient<IAssetDependencyService, AssetDependencyService>()
            .AddTransient<IAssetExtractor, NpmAssetExtractor>()
            .AddTransient<IAssetExtractor, NuGetAssetExtractor>()
            .AddTransient<IAssetExtractor, DockerAssetExtractor>()
            .AddTransient<IAggregateAssetExtractor, AggregateAssetExtractor>();

        private static IServiceCollection RegisterVersionControlServices(this IServiceCollection services) => services
            .AddTransient<IVersionControlService, VersionControlService>()
            .AddTransient<IRepositoryService, RepositoryService>()
            .AddTransient<IRepositoryMatcher, RepositoryMatcher>()
            .AddTransient<IVersionControlProvider, BitBucketProvider>()
            .AddTransient<IVersionControlProvider, GitlabProvider>()
            .AddTransient<IAggregateVersionControlProvider, AggregateVersionControlProvider>();
    }
}
