using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public class GitlabProvider : IVersionControlProvider
    {
        public async Task<IEnumerable<Asset>> GetAssetsAsync(VersionControlDto versionControl, RepositoryDto repository)
        {
            return await Task.FromResult(Enumerable.Empty<Asset>());
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControlDto source)
        {
            return await Task.FromResult(Enumerable.Empty<Repository>());
        }

        public bool Supports(VersionControlKind kind) => kind == VersionControlKind.Gitlab;
    }
}
