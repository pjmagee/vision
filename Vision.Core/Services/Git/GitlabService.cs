using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public class GitlabService : IGitService
    {
        public async Task<IEnumerable<Asset>> GetAssetsAsync(GitRepository repository)
        {
            if (repository.Source.Kind != GitKind.Gitlab) return Enumerable.Empty<Asset>();
            throw new NotImplementedException("TODO");
        }

        public async Task<IEnumerable<GitRepository>> GetRepositoriesAsync(GitSource source)
        {
            if (source.Kind != GitKind.Gitlab) return Enumerable.Empty<GitRepository>();
            throw new NotImplementedException("TODO");
        }
    }
}
