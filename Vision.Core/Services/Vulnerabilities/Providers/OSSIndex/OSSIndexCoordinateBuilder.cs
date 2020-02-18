using System;

namespace Vision.Core
{
    /// <summary>
    /// https://github.com/package-url/purl-spec
    /// https://github.com/package-url
    /// </summary>
    public class OSSIndexCoordinateBuilder
    {
        public Uri GetPackageUrl(DependencyVersion version)
        {
            return version.Dependency.Kind switch
            {
                EcosystemKind.NuGet => new Uri($"pkg:nuget/{version.Dependency.Name}@{version.Version}"),
                EcosystemKind.Npm => new Uri($"pkg:npm/{version.Dependency.Name}@{version.Version}"),
                EcosystemKind.Maven => new Uri($"pkg:maven/{version.Dependency.Name}@{version.Version}"),
                EcosystemKind.RubyGem => new Uri($"pkg:gem/{version.Dependency.Name}@{version.Version}"),
                EcosystemKind.PyPi => new Uri($"pkg:pypi/{version.Dependency.Name}@{version.Version}"),
                EcosystemKind.Docker => new Uri($"pkg:docker/{version.Dependency.Name}@{version.Version}"),
                _ => throw new NotSupportedException($"Could not build OSS Index Package URI because {version.Dependency.Kind} is not supported.")
            };
        }
    }
}