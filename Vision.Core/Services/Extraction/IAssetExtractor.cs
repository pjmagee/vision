using System.Collections.Generic;
using System.Linq;

namespace Vision.Core
{
    public interface IAssetExtractor
    {
        IEnumerable<Extract> ExtractDependencies(Asset asset);
        IEnumerable<Extract> ExtractFrameworks(Asset asset);
    }
}
