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
                DependencyVersion version = null;

                try
                {
                    version = await service.GetLatestMetaDataAsync(dependency);
                }
                catch(Exception e)
                {
                    logger.LogError(e, $"{service.GetType().Name} failed to retrieve version for {dependency.Name}");
                }
                
                if (version != null)
                    return version;
            }

            return new DependencyVersion { IsLatest = false, Version = "UNKNOWN", Dependency = dependency, DependencyId = dependency.Id };
        }

        public bool Supports(DependencyKind kind) => versionServices.Any(s => s.Supports(kind));
    }    
}
