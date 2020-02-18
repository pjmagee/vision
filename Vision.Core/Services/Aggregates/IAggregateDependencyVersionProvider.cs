using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IAggregateDependencyVersionProvider
    {
        Task<DependencyVersion> GetLatestMetaDataAsync(Dependency dependency);
    }
}