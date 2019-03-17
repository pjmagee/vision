using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public class AggregateVersionProvider : IVersionProvider
    {
        private readonly IEnumerable<IVersionProvider> versionServices;
        private readonly ILogger<AggregateVersionProvider> logger;

        public AggregateVersionProvider(NuGetVersionProvider nuGetVersionService, DockerVersionProvider dockerVersionService, NpmVersionProvider nPMVersionService, ILogger<AggregateVersionProvider> logger)
        {
            versionServices = new IVersionProvider[] { nuGetVersionService, dockerVersionService, nPMVersionService };
            this.logger = logger;
        }      

        public async Task<DependencyVersion> GetLatestMetaDataAsync(Dependency dependency)
        {
            foreach(IVersionProvider service in versionServices.Where(service => service.Supports(dependency.Kind)))
            {
                try
                {
                    var latest = await service.GetLatestMetaDataAsync(dependency);

                    if(latest != null)
                    {
                        logger.LogInformation($"Aggregate version service found latest version for {dependency.Name}: {latest}");
                        return latest;
                    }
                }
                catch(Exception e)
                {
                    logger.LogError(e, $"Aggregate version service failed to retrieve version using {service.GetType().Name}");
                }
            }

            return new DependencyVersion { IsLatest = false, Version = "UNKNOWN", Dependency = dependency, DependencyId = dependency.Id };
        }

        public bool Supports(DependencyKind kind) => versionServices.Any(s => s.Supports(kind));
    }    
}
