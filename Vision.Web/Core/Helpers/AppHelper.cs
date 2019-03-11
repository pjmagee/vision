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
        public const string PackagesFile = "packages.json";
        public const string GradleFile = "gradle";

        public static DependencyKind GetDependencyKind(this string path)
        {
            if (path.EndsWith(CsProjFile)) return DependencyKind.NuGet;
            if (path.EndsWith(RequirementsFile)) return DependencyKind.PyPi;
            if (path.EndsWith(PomFile)) return DependencyKind.Maven;
            if (path.EndsWith(GradleFile)) return DependencyKind.Gradle;
            if (path.EndsWith(GemFile)) return DependencyKind.RubyGem;
            if (path.EndsWith(DockerFile)) return DependencyKind.Docker;
            if (path.EndsWith(PackagesFile)) return DependencyKind.Npm;

            throw new Exception("Unsupported file for kind");
        }

        public static IEnumerable<DependencyKind> DependencyKinds { get; } = Enum.GetValues(typeof(DependencyKind)).Cast<DependencyKind>();
        
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
