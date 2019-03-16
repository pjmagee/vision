namespace Vision.Web.Core
{
    using System;

    public static class DependencyKindExtensions
    {
        public static string GetFileExtension(this DependencyKind kind) => kind switch
        {
            DependencyKind.Docker => AppHelper.DockerFile,
            DependencyKind.NuGet => AppHelper.NuGetFile,
            DependencyKind.PyPi => AppHelper.RequirementsFile,
            DependencyKind.Maven => AppHelper.MavenFile,
            DependencyKind.Gradle => AppHelper.GradleFile,
            DependencyKind.RubyGem => AppHelper.RubyGemFile,
            DependencyKind.Npm => AppHelper.NpmFile,
            _ => throw new Exception("Unsupported kind for File extension")
        };
    }
}

