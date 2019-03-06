using System;
using System.Collections.Generic;
using System.Linq;
using Vision.Shared;

namespace Vision.Core
{
    public static class AppHelper
    {
        public const string CsProjFile = "csproj";
        public const string RequirementsFile = "requirements.txt";
        public const string PomFile = "pom.xml";
        public const string GemFile = "GemFile";
        public const string DockerFile = "DockerFile";
        public const string PackagesFile = "packages.json";
        public const string GradleFile = "gradle";

        public static IEnumerable<DependencyKind> DependencyKinds { get; } = Enum.GetValues(typeof(DependencyKind)).Cast<DependencyKind>();

        public static DependencyKind GetDependencyKind(this Asset asset)
        {
            var file = asset.Path;

            if (file.EndsWith(CsProjFile)) return DependencyKind.NuGet;
            if (file.EndsWith(RequirementsFile)) return DependencyKind.PyPi;
            if (file.EndsWith(PomFile)) return DependencyKind.Maven;
            if (file.EndsWith(GradleFile)) return DependencyKind.Gradle;
            if (file.EndsWith(GemFile)) return DependencyKind.RubyGem;
            if (file.EndsWith(DockerFile)) return DependencyKind.Docker;
            if (file.EndsWith(PackagesFile)) return DependencyKind.Npm;           

            throw new Exception("Unsupported file for DependencyKind");
        }

        public static bool Is(this Asset asset, DependencyKind kind) => kind == asset.GetDependencyKind();

        public static bool IsNot(this Asset asset, DependencyKind kind) => !asset.Is(kind);

        public static bool IsSupported(this Asset asset) => asset.Path.IsSupported();
        
        public static bool IsSupported(this string path)
        {
            if (string.IsNullOrEmpty(path)) throw new Exception("Path is null");
            if (path.Contains("node_modules")) return false;
            if (path.EndsWith(CsProjFile)) return true;
            if (path.EndsWith(RequirementsFile)) return true;
            if (path.EndsWith(PomFile)) return true;
            if (path.EndsWith(GemFile)) return true;
            if (path.EndsWith(DockerFile)) return true;
            if (path.EndsWith(PackagesFile)) return true;
            if (path.EndsWith(GradleFile)) return true;

            return false;
        }

        public static string GetFileExtension(this DependencyKind kind)
        {
            switch (kind)
            {
                case DependencyKind.Docker: return DockerFile;
                case DependencyKind.NuGet: return CsProjFile;
                case DependencyKind.PyPi: return RequirementsFile;
                case DependencyKind.Maven: return PomFile;
                case DependencyKind.Gradle: return GradleFile;
                case DependencyKind.RubyGem: return GemFile;
                case DependencyKind.Npm: return PackagesFile;
            }

            throw new Exception("Unsupported kind for File extension");
        }
    }
}
