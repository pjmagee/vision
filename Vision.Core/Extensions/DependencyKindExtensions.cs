namespace Vision.Core
{
    using System;

    public static class EcosystemKindExtensions
    {
        public static string GetFileExtension(this EcosystemKind kind) => kind switch
        {
            EcosystemKind.Docker => Constants.DockerFile,
            EcosystemKind.NuGet => Constants.NuGetFile,
            EcosystemKind.PyPi => Constants.RequirementsFile,
            EcosystemKind.Maven => Constants.MavenFile,
            EcosystemKind.Gradle => Constants.GradleFile,
            EcosystemKind.RubyGem => Constants.RubyGemFile,
            EcosystemKind.Npm => Constants.NpmFile,
            _ => throw new Exception("Unsupported kind for File extension")
        };
    }
}

