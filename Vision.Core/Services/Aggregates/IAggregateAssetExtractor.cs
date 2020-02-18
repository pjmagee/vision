using System.Collections.Generic;

namespace Vision.Core
{
    public interface IAggregateAssetExtractor
    {
        IEnumerable<Extract> ExtractDependencies(Asset asset);
        IEnumerable<Extract> ExtractRuntimes(Asset asset);
        string ExtractPublishName(Asset asset);
    }
}