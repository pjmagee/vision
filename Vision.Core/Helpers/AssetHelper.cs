using System;
using Vision.Shared;

namespace Vision.Core
{
    public static class AssetHelper
    {
        private const string CsProj = "csproj";
        private const string RequirementsTxt = "requirements.txt";
        private const string PomXml = "pom.xml";
        private const string GemFile = "GemFile";
        private const string DockerFile = "DockerFile";
        private const string PackagesJson = "packages.json";
        private const string GradleFile = "gradle";

        public static DependencyKind GetDependencyKind(this Asset asset)
        {
            var file = asset.Path;

            if (file.EndsWith(CsProj)) return DependencyKind.NuGet;
            if (file.EndsWith(RequirementsTxt)) return DependencyKind.PyPi;
            if (file.EndsWith(PomXml)) return DependencyKind.Maven;
            if (file.EndsWith(GradleFile)) return DependencyKind.Gradle;
            if (file.EndsWith(GemFile)) return DependencyKind.RubyGem;
            if (file.EndsWith(DockerFile)) return DependencyKind.Docker;
            if (file.EndsWith(PackagesJson)) return DependencyKind.Npm;
            if (file.EndsWith())

            throw new Exception("Unsupported file for DependencyKind");
        }

        public static bool Is(this Asset asset, DependencyKind kind) => kind == asset.GetDependencyKind();
        public static bool IsNot(this Asset asset, DependencyKind kind) => !asset.Is(kind);
        public static bool IsSupported(this Asset asset) => asset.Path.IsSupported();

        public static bool IsSupported(this string path)
        {
            if (string.IsNullOrEmpty(path)) throw new Exception("Path is null");

            if (path.EndsWith(CsProj)) return true;
            if (path.EndsWith(RequirementsTxt)) return true;
            if (path.EndsWith(PomXml)) return true;
            if (path.EndsWith(GemFile)) return true;
            if (path.EndsWith(DockerFile)) return true;
            if (path.EndsWith(PackagesJson)) return true;
            if (path.EndsWith(GradleFile)) return true;

            return false;
        }

        public static string GetFileExtension(DependencyKind kind)
        {
            switch (kind)
            {
                case DependencyKind.Docker: return DockerFle;
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
