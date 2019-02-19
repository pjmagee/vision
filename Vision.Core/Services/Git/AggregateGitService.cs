using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public class AggregateGitService : IGitService
    {
        private readonly IEnumerable<IGitService> gitServices;

        public AggregateGitService(IEnumerable<IGitService> gitServices)
        {
            this.gitServices = gitServices;
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync(GitRepository repository)
        {
            List<Asset> results = new List<Asset>();

            foreach (var gitService in gitServices)
            {
                IEnumerable<Asset> assets = await gitService.GetAssetsAsync(repository);
                results.AddRange(assets);
            }

            return results;
        }

        public async Task<IEnumerable<GitRepository>> GetRepositoriesAsync(GitSource source)
        {
            List<GitRepository> results = new List<GitRepository>();

            foreach (var gitService in gitServices)
            {
                IEnumerable<GitRepository> repositories = await gitService.GetRepositoriesAsync(source);
                results.AddRange(repositories);
            }

            return results;
        }
    }
}


