namespace Vision.Web.Core
{
    using System.Threading.Tasks;

    public interface IVersionService
    {
        DependencyKind Kind { get;  }
        Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency);
    }
}
