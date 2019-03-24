using System;

namespace Vision.Web.Core
{
    public static class CssClassExtensions
    {
        public static string GetIconClass(this Enum kind) => $"icon {kind.ToString().ToLower()}";

        public static string GetMetricColour(this MetricAlertKind kind) => kind switch
        {
            MetricAlertKind.Standard => "primary",
            MetricAlertKind.Good => "success",
            MetricAlertKind.Warning => "warning",
            MetricAlertKind.Bad => "danger",
            _ => string.Empty
        };

        public static string GetFontAwesomeClass(this CiCdKind kind) => kind switch
        {
            CiCdKind.Gitlab => Brand("fa-gitlab"),
            CiCdKind.Jenkins => Brand("fa-jenkins"),
            CiCdKind.TeamCity => Brand("fa-teamcity"),
            _ => string.Empty
        };
                
        public static string GetFontAwesomeClass(this Metric metric) => 
            metric.DependencyKind.HasValue ? 
            metric.DependencyKind.Value.GetFontAwesomeClass() : 
            metric.CategoryKind.GetFontAwesomeClass();

        private static string GetFontAwesomeClass(this CategoryKind kind) => kind switch
        {
            CategoryKind.Asset => Regular("fa-file-code"),
            CategoryKind.CiCd => Solid("fa-cogs"),
            CategoryKind.Data => Regular("fa-data"),
            CategoryKind.Source => Solid("fa-code-branch"),
            CategoryKind.VersionControl => Solid("fa-server"),
            CategoryKind.Repository => Solid("fa-code-branch"),
            CategoryKind.Dependency => Regular("fa-file-archive"),
            CategoryKind.Framework => Solid("fa-cubes"),
            CategoryKind.Registry => Solid("fa-server"),
            CategoryKind.Version => Regular("fa-copy"),
            _ => string.Empty
        };

        private static string GetFontAwesomeClass(this DependencyKind kind) => kind switch
        {
            DependencyKind.Docker => Brand("fa-docker"),
            DependencyKind.Gradle => Brand("fa-java"),
            DependencyKind.Maven => Brand("fa-java"),
            DependencyKind.Npm => Brand("fa-npm"),
            DependencyKind.NuGet => Brand("fa-microsoft"),
            DependencyKind.PyPi => Brand("fa-python"),
            DependencyKind.RubyGem => Solid("fa-gem"),
            _ => string.Empty
        };

        private static string Regular(string fa) => $"far {fa}";
        private static string Solid(string fa) => $"fas {fa}";
        private static string Brand(string fa) => $"fab {fa}";
    }
}

