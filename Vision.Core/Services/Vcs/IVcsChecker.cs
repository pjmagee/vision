using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IVcsChecker
    {
        Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControl source);
        Task<IEnumerable<Asset>> GetAssetsAsync(Repository repository);
    }
}
