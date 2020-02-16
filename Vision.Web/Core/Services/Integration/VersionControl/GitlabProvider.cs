using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public class GitlabProvider : IVersionControlProvider
    {
        public VersionControlKind Kind { get; } = VersionControlKind.Gitlab;

        public async Task<IEnumerable<Asset>> GetAssetsAsync(VersionControlDto versionControl, RepositoryDto repository)
        {
            return await Task.FromResult(Enumerable.Empty<Asset>());
        }

        public async Task<IEnumerable<VcsRepository>> GetRepositoriesAsync(VersionControlDto source)
        {
            return await Task.FromResult(Enumerable.Empty<VcsRepository>());
        }
    }
}
