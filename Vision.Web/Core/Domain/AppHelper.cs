using System;
using System.Collections.Generic;
using System.Linq;

namespace Vision.Web.Core
{
    public static class AppHelper
    {
        public static IEnumerable<CiCdKind> CiCdKinds { get; } = Enum.GetValues(typeof(CiCdKind)).Cast<CiCdKind>();

        public static IEnumerable<EcosystemKind> EcosystemKinds = Enum.GetValues(typeof(EcosystemKind)).Cast<EcosystemKind>();
        public static IEnumerable<VcsKind> VcsKinds { get; } = Enum.GetValues(typeof(VcsKind)).Cast<VcsKind>();
        public static string GetName(this Enum kind) => Enum.GetName(kind.GetType(), kind);

        public const string NuGetFile = "csproj";
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
