using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public class AggregateDependencyVersionProvider : IAggregateDependencyVersionProvider
    {
        private readonly IEnumerable<IDependencyVersionProvider> providers;
        private readonly IRegistryService registryService;
        private readonly IDataProtector protector;
        private readonly IMemoryCache cache;
        private readonly ILogger<AggregateDependencyVersionProvider> logger;

        public AggregateDependencyVersionProvider(
            IRegistryService registryService,
            IDataProtectionProvider dataProtectorProvider,
            IMemoryCache cache,
            IEnumerable<IDependencyVersionProvider> providers,
            ILogger<AggregateDependencyVersionProvider> logger)
        {
            this.registryService = registryService;
            this.cache = cache;
            this.logger = logger;
            this.protector = dataProtectorProvider.CreateProtector("REGISTRY");
            this.providers = providers;
        }

        public async Task<DependencyVersion> GetLatestMetaDataAsync(Dependency dependency)
        {
            using (var scope = logger.BeginScope($"{nameof(GetLatestMetaDataAsync)}::[{dependency.Name}]::SEARCH"))
            {
                if (!cache.TryGetValue(dependency.Name, out DependencyVersion version))
                {
                    version = await GetVersionByProviders(dependency);
                }

                return await Task.FromResult(version);
            }
        }

        private async Task<DependencyVersion> GetVersionByProviders(Dependency dependency)
        {
            List<RegistryDto> registries = await registryService.GetEnabledByKindAsync(dependency.Kind);
            DependencyVersion version = null;

            foreach (IDependencyVersionProvider service in providers.Where(service => service.Supports(dependency.Kind)))
            {
                foreach (RegistryDto registry in registries)
                {
                    try
                    {
                        version = await GetVersionByProviderAndRegistry(dependency, service, registry);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"{nameof(GetVersionByProviders)}::[{dependency.Name}]::[{registry.Endpoint}]::ERROR");
                    }
                }

                if (version != null) break;
            }

            if (version == null)
            {
                logger.LogInformation($"{nameof(GetVersionByProviders)}::[{dependency.Name}]::SEARCH::FAIL");
                version = new DependencyVersion { IsLatest = false, Version = "UNKNOWN", Dependency = dependency, DependencyId = dependency.Id };
            }

            cache.Set(dependency.Id, version, absoluteExpiration: DateTimeOffset.Now.AddHours(24));

            return version;
        }

        private async Task<DependencyVersion> GetVersionByProviderAndRegistry(Dependency dependency, IDependencyVersionProvider service, RegistryDto registry)
        {
            registry.ApiKey = protector.Unprotect(registry.ApiKey);
            registry.Username = protector.Unprotect(registry.Username);
            registry.Password = protector.Unprotect(registry.Password);

            logger.LogInformation($"{nameof(GetVersionByProviderAndRegistry)}::[{dependency.Name}]::[{registry.Endpoint}]::SEARCH::");

            DependencyVersion version = null;

            using (var serviceScope = logger.BeginScope($"{nameof(GetVersionByProviderAndRegistry)}::[{service.GetType().Name}]::SEARCH"))
            {
                version = await service.GetLatestMetaDataAsync(registry, dependency);
            }

            if (version != null)
            {
                logger.LogInformation($"{nameof(GetVersionByProviderAndRegistry)}::[{dependency.Name}]::[{registry.Endpoint}]::SUCCESS::[{version.Version}]");
            }
            else
            {
                logger.LogInformation($"{nameof(GetVersionByProviderAndRegistry)}::[{dependency.Name}]::[{registry.Endpoint}]::FAIL");
            }

            return version;
        }
    }
}
