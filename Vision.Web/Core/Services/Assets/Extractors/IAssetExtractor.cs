using System.Collections.Generic;

namespace Vision.Web.Core
{
    public interface IAssetExtractor
    {
        EcosystemKind Kind { get; }
        IEnumerable<Extract> ExtractDependencies(Asset asset);
        IEnumerable<Extract> ExtractEcoSystem(Asset asset);
        string ExtractPublishName(Asset asset);
    }
}
