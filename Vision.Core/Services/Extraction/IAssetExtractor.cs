using System.Collections.Generic;

namespace Vision.Core
{

    public interface IAssetExtractor
    {
        IEnumerable<Extract> ExtractDependencies(Asset asset);
        IEnumerable<Extract> ExtractFrameworks(Asset asset);
    }
}
