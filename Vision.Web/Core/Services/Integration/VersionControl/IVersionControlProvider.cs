using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IVersionControlProvider
    {
        Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControlDto versionControl);
        Task<IEnumerable<Asset>> GetAssetsAsync(VersionControlDto versionControl, RepositoryDto repository);
        bool Supports(VersionControlKind kind);
    }
}
