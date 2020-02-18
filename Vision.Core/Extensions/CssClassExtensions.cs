using System;

namespace Vision.Core
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
            metric.EcosystemKind.HasValue ?
            metric.EcosystemKind.Value.GetFontAwesomeClass() :
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
            CategoryKind.Ecosystem => Solid("fa-cubes"),
            CategoryKind.EcosystemVersion => Regular("fa-copy"),
            CategoryKind.Registry => Solid("fa-server"),
            _ => string.Empty
        };

        private static string GetFontAwesomeClass(this EcosystemKind kind) => kind switch
        {
            EcosystemKind.Docker => Brand("fa-docker"),
            EcosystemKind.Gradle => Brand("fa-java"),
            EcosystemKind.Maven => Brand("fa-java"),
            EcosystemKind.Npm => Brand("fa-npm"),
            EcosystemKind.NuGet => Brand("fa-microsoft"),
            EcosystemKind.PyPi => Brand("fa-python"),
            EcosystemKind.RubyGem => Solid("fa-gem"),
            _ => string.Empty
        };

        private static string Regular(string fa) => $"far {fa}";
        private static string Solid(string fa) => $"fas {fa}";
        private static string Brand(string fa) => $"fab {fa}";
    }
}

