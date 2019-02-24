using System;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public class AggregateVersionChecker : IVersionChecker
    {
        public DependencyKind Kind { get; }

        public Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency)
        {
            throw new NotImplementedException();
        }
    }
}
