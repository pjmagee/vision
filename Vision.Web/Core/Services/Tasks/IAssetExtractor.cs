using System.Collections.Generic;

namespace Vision.Web.Core
{

    public interface IAssetExtractor
    {
        bool Supports(DependencyKind kind);
        IEnumerable<Extract> ExtractDependencies(Asset asset);
        IEnumerable<Extract> ExtractFrameworks(Asset asset);
    }
}
