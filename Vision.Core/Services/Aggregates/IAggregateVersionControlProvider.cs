using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IAggregateVersionControlProvider
    {
        Task<IEnumerable<Asset>> GetAssetsAsync(VersionControlDto versionControl, RepositoryDto repository);
        Task<IEnumerable<VcsRepository>> GetRepositoriesAsync(VersionControlDto source);
    }
}