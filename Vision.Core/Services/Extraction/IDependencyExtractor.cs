using System.Collections.Generic;
using System.Linq;

namespace Vision.Core
{
    public interface IDependencyExtractor
    {
        IEnumerable<Extract> ExtractDependencies(Asset asset);
        IEnumerable<Extract> ExtractFrameworks(Asset asset);
    }
}
