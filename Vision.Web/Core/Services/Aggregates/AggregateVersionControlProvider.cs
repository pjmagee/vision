using Microsoft.AspNetCore.DataProtection;
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
        private readonly ILogger<AggregateVersionControlProvider> logger;
        
        public AggregateVersionControlProvider(IEnumerable<IVersionControlProvider> providers, ILogger<AggregateVersionControlProvider> logger)
        {
            this.providers = providers;      
            this.logger = logger;
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync(VersionControlDto versionControl, RepositoryDto repository)
        {
            using (var scope = logger.BeginScope($"{nameof(GetAssetsAsync)}::[{repository.Url}]"))
            {
                List<Asset> results = new List<Asset>();

                foreach (IVersionControlProvider provider in providers.Where(p => p.Supports(versionControl.Kind)))
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

        public async Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControlDto versionControl)
        {
            List<Repository> results = new List<Repository>();

            foreach (IVersionControlProvider provider in providers.Where(provider => provider.Supports(versionControl.Kind)))
            {
                IEnumerable<Repository> repositories = await provider.GetRepositoriesAsync(versionControl);
                results.AddRange(repositories);
            }

            logger.LogInformation($"{nameof(GetRepositoriesAsync)}::[{versionControl.Endpoint}]::RESULTS::{results.Count}");

            return results;
        }
    }
}


