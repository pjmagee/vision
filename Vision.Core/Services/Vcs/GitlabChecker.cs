using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public class GitlabChecker : IVcsChecker
    {
        public async Task<IEnumerable<Asset>> GetAssetsAsync(Repository repository)
        {
            if (repository.VersionControl.Kind != VersionControlKind.Gitlab) return await Task.FromResult(Enumerable.Empty<Asset>());
            throw new NotImplementedException("TODO");
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControl source)
        {
            if (source.Kind != VersionControlKind.Gitlab) return await Task.FromResult(Enumerable.Empty<Repository>());
            throw new NotImplementedException("TODO");
        }
    }
}
