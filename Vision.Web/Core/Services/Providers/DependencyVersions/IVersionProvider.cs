using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IVersionProvider
    {
        bool Supports(DependencyKind kind);
        Task<DependencyVersion> GetLatestMetaDataAsync(Dependency dependency);
    }
}
