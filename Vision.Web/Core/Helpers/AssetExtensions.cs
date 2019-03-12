namespace Vision.Web.Core
{
    using System;

    public static class AssetExtensions
    {
        //public const string CsProjFile = "csproj";
        //public const string RequirementsFile = "requirements.txt";
        //public const string PomFile = "pom.xml";
        //public const string GemFile = "GemFile";
        //public const string DockerFile = "DockerFile";
        //public const string PackageJsonFile = "package.json";
        //public const string GradleFile = "gradle";

        public static DependencyKind GetDependencyKind(this Asset asset)
        {
            string file = asset.Path;

            if (file.EndsWith(AppHelper.CsProjFile)) return DependencyKind.NuGet;
            if (file.EndsWith(AppHelper.RequirementsFile)) return DependencyKind.PyPi;
            if (file.EndsWith(AppHelper.PomFile)) return DependencyKind.Maven;
            if (file.EndsWith(AppHelper.GradleFile)) return DependencyKind.Gradle;
            if (file.EndsWith(AppHelper.GemFile)) return DependencyKind.RubyGem;
            if (file.EndsWith(AppHelper.DockerFile)) return DependencyKind.Docker;
            if (file.EndsWith(AppHelper.PackageJsonFile)) return DependencyKind.Npm;

            throw new Exception("Unsupported file for DependencyKind");
        }

        public static bool Is(this Asset asset, DependencyKind kind) => kind == asset.GetDependencyKind();

        public static bool IsNot(this Asset asset, DependencyKind kind) => !asset.Is(kind);

        public static bool IsSupported(this Asset asset) => asset.Path.IsSupported();

        public static bool IsSupported(this string path)
        {
            if (path.Contains("node_modules")) return false;


            if (path.EndsWith(AppHelper.CsProjFile)) return true;
            if (path.EndsWith(AppHelper.PackageJsonFile)) return true;            
            
            if (path.EndsWith(AppHelper.RequirementsFile)) return true;
            if (path.EndsWith(AppHelper.PomFile)) return true;
            if (path.EndsWith(AppHelper.GemFile)) return true;
            if (path.EndsWith(AppHelper.DockerFile)) return true;            
            if (path.EndsWith(AppHelper.GradleFile)) return true;

            return false;
        }
    }
}

