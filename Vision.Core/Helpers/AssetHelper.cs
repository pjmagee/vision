using System;
using Vision.Shared;

namespace Vision.Core
{
    public static class AssetHelper
    {
        public const string CsProj = "csproj";
        public const string RequirementsTxt = "requirements.txt";
        public const string PomXml = "pom.xml";
        public const string GemFile = "GemFile";
        public const string DockerFile = "DockerFile";
        public const string PackagesJson = "packages.json";

        public static DependencyKind GetDependencyKind(this Asset asset)
        {
            var file = asset.Path;

            if (file.EndsWith(CsProj)) return DependencyKind.NuGet;
            if (file.EndsWith(RequirementsTxt)) return DependencyKind.PyPi;
            if (file.EndsWith(PomXml)) return DependencyKind.Maven;
            if (file.EndsWith(GemFile)) return DependencyKind.RubyGem;
            if (file.EndsWith(DockerFile)) return DependencyKind.Docker;
            if (file.EndsWith(PackagesJson)) return DependencyKind.Npm;

            throw new Exception("Unsupported file for DependencyKind");
        }

        public static bool Is(this Asset asset, DependencyKind kind) => kind == asset.GetDependencyKind();
        public static bool IsNot(this Asset asset, DependencyKind kind) => !asset.Is(kind);
        public static bool IsSupported(this Asset asset) => asset.Path.IsSupported();

        public static bool IsSupported(this string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Path is null");

            if (path.EndsWith(CsProj)) return true;
            if (path.EndsWith(RequirementsTxt)) return true;
            if (path.EndsWith(PomXml)) return true;
            if (path.EndsWith(GemFile)) return true;
            if (path.EndsWith(DockerFile)) return true;
            if (path.EndsWith(PackagesJson)) return true;

            return false;
        }

        public static string GetFileExtension(DependencyKind kind)
        {
            if (kind == DependencyKind.NuGet) return CsProj;
            if (kind == DependencyKind.PyPi) return RequirementsTxt;
            if (kind == DependencyKind.Maven) return PomXml;
            if (kind == DependencyKind.RubyGem) return GemFile;
            if (kind == DependencyKind.Docker) return DockerFile;
            if (kind == DependencyKind.Npm) return PackagesJson;

            throw new Exception("Unsupported kind for File extension");
        }
    }
}
