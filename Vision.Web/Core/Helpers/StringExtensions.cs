namespace Vision.Web.Core
{
    using System;

    public static class StringExtensions
    {
        public static DependencyKind GetDependencyKind(this string path)
        {
            if (path.EndsWith(AppHelper.CSharpProjectFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.NuGet;
            if (path.EndsWith(AppHelper.PythonRequirementsFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.PyPi;
            if (path.EndsWith(AppHelper.MavenPomFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.Maven;
            if (path.EndsWith(AppHelper.GradleFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.Gradle;
            if (path.EndsWith(AppHelper.RubyGemFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.RubyGem;
            if (path.EndsWith(AppHelper.DockerFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.Docker;
            if (path.EndsWith(AppHelper.NodePackageFile, StringComparison.CurrentCultureIgnoreCase)) return DependencyKind.Npm;

            throw new Exception("Unsupported file for kind");
        }

        public static bool IsSupported(this string path)
        {
            if (path.Contains("node_modules")) return false;
            if (path.EndsWith(AppHelper.CSharpProjectFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(AppHelper.NodePackageFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(AppHelper.PythonRequirementsFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(AppHelper.MavenPomFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(AppHelper.RubyGemFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(AppHelper.DockerFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (path.EndsWith(AppHelper.GradleFile, StringComparison.CurrentCultureIgnoreCase)) return true;
            return false;
        }
    }
}
