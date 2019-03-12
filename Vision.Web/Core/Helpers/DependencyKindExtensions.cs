namespace Vision.Web.Core
{
    using System;

    public static class DependencyKindExtensions
    {
        public static string GetFileExtension(this DependencyKind kind) => kind switch
        {
            DependencyKind.Docker => AppHelper.DockerFile,
            DependencyKind.NuGet => AppHelper.CSharpProjectFile,
            DependencyKind.PyPi => AppHelper.PythonRequirementsFile,
            DependencyKind.Maven => AppHelper.MavenPomFile,
            DependencyKind.Gradle => AppHelper.GradleFile,
            DependencyKind.RubyGem => AppHelper.RubyGemFile,
            DependencyKind.Npm => AppHelper.NodePackageFile,
            _ => throw new Exception("Unsupported kind for File extension")
        };
    }
}

