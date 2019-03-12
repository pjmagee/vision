namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class AppHelper
    {
        public const string CsProjFile = "csproj";
        public const string RequirementsFile = "requirements.txt";
        public const string PomFile = "pom.xml";
        public const string GemFile = "GemFile";
        public const string DockerFile = "DockerFile";
        public const string PackageJsonFile = "package.json";
        public const string GradleFile = "gradle";

        public static DependencyKind GetDependencyKind(this string path)
        {
            if (path.EndsWith(CsProjFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.NuGet;
            if (path.EndsWith(RequirementsFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.PyPi;
            if (path.EndsWith(PomFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.Maven;
            if (path.EndsWith(GradleFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.Gradle;
            if (path.EndsWith(GemFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.RubyGem;
            if (path.EndsWith(DockerFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.Docker;
            if (path.EndsWith(PackageJsonFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.Npm;

            throw new Exception("Unsupported file for kind");
        }

        public static IEnumerable<DependencyKind> DependencyKinds { get; } = Enum.GetValues(typeof(DependencyKind)).Cast<DependencyKind>();

        public static bool IsSupported(this string path)
        {
            if (path.Contains("node_modules")) return false;
            if (path.EndsWith(CsProjFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(PackageJsonFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(RequirementsFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(PomFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(GemFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(DockerFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(GradleFile, StringComparison.CurrentCultureIgnoreCase)) return true;
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
                case DependencyKind.Npm: return PackageJsonFile;
            }

            throw new Exception("Unsupported kind for File extension");
        }
    }
}
