using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public class AggregateCICDProvider : ICICDProvider
    {
        private readonly VisionDbContext context;
        private readonly ILogger<AggregateCICDProvider> logger;
        private readonly IEnumerable<ICICDProvider> providers;

        public AggregateCICDProvider(VisionDbContext context, IEnumerable<ICICDProvider> providers, ILogger<AggregateCICDProvider> logger)
        {
            this.context = context;
            this.logger = logger;
            this.providers = providers;
        }

        public async Task<List<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
        {
            Repository repository = context.Repositories.Find(repositoryId);
            List<CiCdBuildDto> builds = new List<CiCdBuildDto>();

            foreach (var service in providers)
            {
                var results = await service.GetBuildsByRepositoryIdAsync(repositoryId);
                builds.AddRange(results);
            }
            
            logger.LogInformation($"Found {builds.Count} for {repository.WebUrl}");

            return builds;
        }

        public bool Supports(CiCdKind Kind) => providers.Any(service => service.Supports(Kind));
    }
}
