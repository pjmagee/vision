using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public class AggregateCICDBuildsProvider : IAggregateCICDBuildsProvider
    {
        private readonly IRepositoryService repositoryService;
        private readonly ICiCdService ciCdService;
        private readonly IEncryptionService encryption;
        private readonly IMemoryCache cache;
        private readonly ILogger<AggregateCICDBuildsProvider> logger;
        private readonly IEnumerable<ICiCdProvider> providers;

        public AggregateCICDBuildsProvider(
            IRepositoryService repositoryService,
            ICiCdService ciCdService,
            IEncryptionService encryption,
            IMemoryCache cache,
            IEnumerable<ICiCdProvider> providers,
            ILogger<AggregateCICDBuildsProvider> logger)
        {
            this.repositoryService = repositoryService;
            this.ciCdService = ciCdService;
            this.encryption = encryption;
            this.cache = cache;
            this.logger = logger;
            this.providers = providers;
        }

        public async Task<List<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
        {
            RepositoryDto repository = await repositoryService.GetByIdAsync(repositoryId);

            using (var scope = logger.BeginScope($"{nameof(GetBuildsByRepositoryIdAsync)}::[{repository.Url}]::CICD::SEARCH"))
            {
                if (!cache.TryGetValue(repositoryId, out List<CiCdBuildDto> builds))
                {
                    builds = new List<CiCdBuildDto>();

                    IPaginatedList<CiCdDto> cicds = await ciCdService.GetAsync(pageIndex: 1, pageSize: 1000);

                    foreach(var cicd in cicds)
                    {
                        encryption.Decrypt(cicd);
                    }

                    foreach (ICiCdProvider provider in providers)
                    {
                        foreach (CiCdDto cicd in cicds.Where(cicd => provider.Supports(cicd.Kind)))
                        {
                            try
                            {
                                List<CiCdBuildDto> results = await provider.GetBuildsAsync(repository, cicd);
                                builds.AddRange(results);
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e, $"{nameof(GetBuildsByRepositoryIdAsync)}::[{repository.Url}]::[{cicd.Endpoint}]::ERROR");
                            }
                        }
                    }

                    logger.LogInformation($"{nameof(GetBuildsByRepositoryIdAsync)}::[{repository.WebUrl}]::CICD::SEARCH::RESULTS::[{builds.Count}]");

                    cache.Set(repositoryId, builds, absoluteExpiration: DateTimeOffset.Now.AddHours(2));
                }

                return await Task.FromResult(builds);
            }
        }
    }
}
