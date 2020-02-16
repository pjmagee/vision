using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IDependencyVersionProvider
    {
        bool Supports(EcosystemKind kind);
        Task<DependencyVersion> GetLatestMetaDataAsync(RegistryDto registry, Dependency dependency);
    }
}
