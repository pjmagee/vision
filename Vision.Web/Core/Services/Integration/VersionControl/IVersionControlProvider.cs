using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IVersionControlProvider
    {
        Task<IEnumerable<VcsRepository>> GetRepositoriesAsync(VersionControlDto versionControl);
        Task<IEnumerable<Asset>> GetAssetsAsync(VersionControlDto versionControl, RepositoryDto repository);
        VersionControlKind Kind { get; }
    }
}
