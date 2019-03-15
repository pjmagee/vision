namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class AppHelper
    {
        public static IEnumerable<DependencyKind> DependencyKinds { get; } = Enum.GetValues(typeof(DependencyKind)).Cast<DependencyKind>();

        public const string CSharpProjectFile = ".csproj";
        public const string PythonRequirementsFile = "requirements.txt";
        public const string MavenPomFile = "pom.xml";
        public const string RubyGemFile = "GemFile";
        public const string DockerFile = "Dockerfile";
        public const string NodePackageFile = "package.json";
        public const string GradleFile = "gradle";

        public static IEnumerable<string> SupportedExtensions { get; } = new List<string>()
        {
            CSharpProjectFile,
            PythonRequirementsFile,
            MavenPomFile,
            RubyGemFile,
            DockerFile,
            NodePackageFile,
            GradleFile
        };
    }
}
