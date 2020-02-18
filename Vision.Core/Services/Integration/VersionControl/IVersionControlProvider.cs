using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IVersionControlProvider
    {
        Task<IEnumerable<VcsRepository>> GetRepositoriesAsync(VersionControlDto versionControl);
        Task<IEnumerable<Asset>> GetAssetsAsync(VersionControlDto versionControl, RepositoryDto repository);
        VcsKind Kind { get; }
    }
}
