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
                    .UseSqlServer(configuration["ConnectionStrings:Default"]), ServiceLifetime.Transient);

            services
                .RegisterVersionControlServices()
                .RegisterAssetServices()
                .RegisterDependencyServices()
                .RegisterCiCdServices()
                .RegisterRazorComponentServices()
                // .RegisterRefreshServices()
                .AddSingleton<IEncryptionService, EncryptionService>()
                .AddScoped<FakeDataGenerator>()
                .AddScoped<IMetricService, MetricService>();

            return services;
        }

        public static IServiceCollection RegisterRefreshServices(this IServiceCollection services) => services
            .AddScoped<ISystemTaskService, SystemTaskService>()
            .AddScoped<IRefreshService, RefreshService>()
            .AddHostedService<BackgroundSystemRefreshMonitor>();

        private static IServiceCollection RegisterRazorComponentServices(this IServiceCollection services) => services
            .AddScoped<SvgService>()
            .AddScoped<NavigationService>();

        private static IServiceCollection RegisterCiCdServices(this IServiceCollection services) => services
            .AddScoped<ICiCdService, CiCdService>()
            .AddScoped<ICiCdProvider, TeamCityProvider>()
            .AddScoped<ICiCdProvider, JenkinsProvider>()
            .AddScoped<IAggregateCICDBuildsProvider, AggregateCICDBuildsProvider>();

        private static IServiceCollection RegisterDependencyServices(this IServiceCollection services) => services
            .AddScoped<IRegistryService, RegistryService>()
            .AddScoped<IDependencyVersionService, DependencyVersionService>()
            .AddScoped<IDependencyService, DependencyService>()
            .AddScoped<IDependencyVersionProvider, NuGetVersionProvider>()
            .AddScoped<IDependencyVersionProvider, DockerVersionProvider>()
            .AddScoped<IDependencyVersionProvider, NpmVersionProvider>()
            .AddScoped<IAggregateDependencyVersionProvider, AggregateDependencyVersionProvider>();

        private static IServiceCollection RegisterAssetServices(this IServiceCollection services) => services
            .AddScoped<IFrameworkService, FrameworkService>()
            .AddScoped<IAssetService, AssetService>()
            .AddScoped<IAssetDependencyService, AssetDependencyService>()
            .AddScoped<IAssetExtractor, NpmAssetExtractor>()
            .AddScoped<IAssetExtractor, NuGetAssetExtractor>()
            .AddScoped<IAssetExtractor, DockerAssetExtractor>()
            .AddScoped<IAggregateAssetExtractor, AggregateAssetExtractor>();

        private static IServiceCollection RegisterVersionControlServices(this IServiceCollection services) => services
            .AddScoped<IVersionControlService, VersionControlService>()
            .AddScoped<IRepositoryService, RepositoryService>()
            .AddScoped<IRepositoryMatcher, RepositoryMatcher>()
            .AddScoped<IVersionControlProvider, BitBucketProvider>()
            .AddScoped<IVersionControlProvider, GitlabProvider>()
            .AddScoped<IAggregateVersionControlProvider, AggregateVersionControlProvider>();
    }
}
