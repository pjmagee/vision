namespace Vision.Web.Core
{
    using System;

    public static class EcosystemKindExtensions
    {
        public static string GetFileExtension(this EcosystemKind kind) => kind switch
        {
            EcosystemKind.Docker => AppHelper.DockerFile,
            EcosystemKind.NuGet => AppHelper.NuGetFile,
            EcosystemKind.PyPi => AppHelper.RequirementsFile,
            EcosystemKind.Maven => AppHelper.MavenFile,
            EcosystemKind.Gradle => AppHelper.GradleFile,
            EcosystemKind.RubyGem => AppHelper.RubyGemFile,
            EcosystemKind.Npm => AppHelper.NpmFile,
            _ => throw new Exception("Unsupported kind for File extension")
        };
    }
}

