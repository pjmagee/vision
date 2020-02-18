using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IDependencyVersionProvider
    {
        bool Supports(EcosystemKind kind);
        Task<DependencyVersion> GetLatestMetaDataAsync(RegistryDto registry, Dependency dependency);
    }
}
