using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public class AggregateCICDBuildsService : ICICDBuildsService
    {
        private readonly VisionDbContext context;
        private readonly ILogger<AggregateCICDBuildsService> logger;
        private readonly IEnumerable<ICICDBuildsService> buildServices;

        public AggregateCICDBuildsService(VisionDbContext context, JenkinsBuildsService jenkinsBuildsService, TeamCityBuildsService teamCityBuildsService, ILogger<AggregateCICDBuildsService> logger)
        {
            this.context = context;
            this.logger = logger;
            this.buildServices = new ICICDBuildsService[] { jenkinsBuildsService, teamCityBuildsService };
        }

        public async Task<List<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
        {
            Repository repository = context.Repositories.Find(repositoryId);

            List<CiCdBuildDto> builds = new List<CiCdBuildDto>();

            foreach (var service in buildServices)
            {
                var results = await service.GetBuildsByRepositoryIdAsync(repositoryId);
                builds.AddRange(results);
            }
            
            logger.LogInformation($"Found {builds.Count} for {repository.WebUrl}");

            return builds;
        }

        public bool Supports(CiCdKind Kind) => buildServices.Any(service => service.Supports(Kind));
    }
}
