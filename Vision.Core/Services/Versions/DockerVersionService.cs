using System;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public class DockerVersionService : IVersionChecker
    {
        public DependencyKind Kind => DependencyKind.Docker;

        public Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency)
        {
            throw new NotImplementedException();
        }
    }
}
