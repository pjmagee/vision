using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public class AggregateVersionControlProvider : IAggregateVersionControlProvider
    {
        private readonly IEnumerable<IVersionControlProvider> providers;
        private readonly IVersionControlService versionControlService;
        private readonly ILogger<AggregateVersionControlProvider> logger;

        public AggregateVersionControlProvider(
            IVersionControlService versionControlService,
            IEnumerable<IVersionControlProvider> providers,
            ILogger<AggregateVersionControlProvider> logger)
        {
            this.providers = providers;
            this.versionControlService = versionControlService;
            this.logger = logger;
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync(Repository repository)
        {
            var versionControl = await versionControlService.GetByIdAsync(repository.VersionControlId);

            using (var scope = logger.BeginScope($"{nameof(GetAssetsAsync)}::[{repository.Url}]"))
            {
                List<Asset> results = new List<Asset>();

                foreach (IVersionControlProvider provider in providers.Where(p => p.Supports(repository.VersionControl.Kind)))
                {
                    try
                    {
                        IEnumerable<Asset> assets = await provider.GetAssetsAsync(versionControl, repository);
                        results.AddRange(assets);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"{nameof(GetAssetsAsync)}::[{repository.Url}]::ERROR");
                    }

                }

                logger.LogInformation($"{nameof(GetAssetsAsync)}::[{repository.Url}]::RESULTS::{results.Count}");

                return results;
            }
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControl source)
        {
            List<Repository> results = new List<Repository>();

            foreach (IVersionControlProvider provider in providers.Where(provider => provider.Supports(source.Kind)))
            {
                IEnumerable<Repository> repositories = await provider.GetRepositoriesAsync(source);
                results.AddRange(repositories);
            }

            logger.LogInformation($"{nameof(GetRepositoriesAsync)}::[{source.Endpoint}]::RESULTS::{results.Count}");

            return results;
        }
    }
}


