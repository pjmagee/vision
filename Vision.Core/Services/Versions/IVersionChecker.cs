using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public interface IVersionChecker
    {
        DependencyKind Kind { get;  }
        Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency);
    }
}
