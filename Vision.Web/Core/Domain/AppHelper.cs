namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class AppHelper
    {
        public static IEnumerable<DependencyKind> DependencyKinds { get; } = Enum.GetValues(typeof(DependencyKind)).Cast<DependencyKind>();

        public const string NuGetFile = ".csproj";
        public const string RequirementsFile = "requirements.txt";
        public const string MavenFile = "pom.xml";
        public const string RubyGemFile = "GemFile";
        public const string DockerFile = "Dockerfile";
        public const string NpmFile = "package.json";
        public const string GradleFile = "gradle";

        public static IEnumerable<string> SupportedExtensions { get; } = new List<string>()
        {
            NuGetFile,
            RequirementsFile,
            MavenFile,
            RubyGemFile,
            DockerFile,
            NpmFile,
            GradleFile
        };
    }
}
