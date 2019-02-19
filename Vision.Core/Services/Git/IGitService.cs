using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IGitService
    {
        Task<IEnumerable<GitRepository>> GetRepositoriesAsync(GitSource source);
        Task<IEnumerable<Asset>> GetAssetsAsync(GitRepository repository);
    }
}
