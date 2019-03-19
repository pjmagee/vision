using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IAggregateVersionControlProvider
    {
        Task<IEnumerable<Asset>> GetAssetsAsync(Repository repository);
        Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControl source);
    }
}