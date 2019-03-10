using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IVersionControlService
    {
        Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControl source);
        Task<IEnumerable<Asset>> GetAssetsAsync(Repository repository);
    }
}
