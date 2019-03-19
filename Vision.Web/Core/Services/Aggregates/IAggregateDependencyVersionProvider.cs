using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IAggregateDependencyVersionProvider
    {
        Task<DependencyVersion> GetLatestMetaDataAsync(Dependency dependency);
    }
}