using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vision.Core
{
    public static class VisionServices
    {
        public static IServiceCollection AddVisionHostedBackgroundServices(this IServiceCollection services)
        {
            return services.AddHostedService<BackgroundSystemRefreshMonitor>();
        }

        public static IServiceCollection AddVisionDbContext(this IServiceCollection services, IConfiguration configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            return services
                     .AddDbContext<VisionDbContext>(options => options
                         .UseLazyLoadingProxies(useLazyLoadingProxies: true)
                         .UseSqlServer(configuration["ConnectionStrings:Default"]), serviceLifetime, serviceLifetime);
        }

        public static IServiceCollection AddVisionServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .RegisterVersionControlServices()
                .RegisterAssetServices()
                .RegisterDependencyServices()
                .RegisterCiCdServices()
                .RegisterVulnerabilityServices()
                .RegisterTaskServices()
                .AddSingleton<IEncryptionService, EncryptionService>()
                .AddTransient<FakeDataGenerator>()
                .AddTransient<IMetricService, MetricService>();

            return services;
        }

        public static IServiceCollection RegisterTaskServices(this IServiceCollection services) =>
            services
               .AddScoped<ISystemTaskService, SystemTaskService>()
               .AddScoped<IRefreshService, RefreshService>();

        public static IServiceCollection RegisterVulnerabilityServices(this IServiceCollection services) => services
            .AddScoped<IVulnerabilityService, VulnerabilityService>()
            .AddScoped<IVulnerabilityProvider, OSSIndexVulnerabilityProvider>()
            .AddScoped<OSSIndexCoordinateBuilder>()
            .AddScoped<IAggregrateVulnerabilityReportProvider, AggregrateVulnerabilityReportProvider>();

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
            .AddTransient<IEcosystemService, EcosystemService>()
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
