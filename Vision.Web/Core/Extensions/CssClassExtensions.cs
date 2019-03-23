namespace Vision.Web.Core
{
    public static class CssClassExtensions
    {
        public static string GetIconClass(this CiCdKind kind) => $"icon {kind.ToString()}";
        public static string GetIconClass(this MetricCategoryKind kind) => $"icon {kind.ToString()}";
        public static string GetIconClass(this DependencyKind kind) => $"icon {kind.ToString()}";
        public static string GetIconClass(this MetricAlertKind kind) => $"icon {kind.ToString()}";
        public static string GetIconClass(this VersionControlKind kind) => $"icon {kind.ToString()}";

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

        private static string GetFontAwesomeClass(this MetricCategoryKind kind) => kind switch
        {
            MetricCategoryKind.Asset => Regular("fa-file-code"),
            MetricCategoryKind.CiCd => Solid("fa-cogs"),
            MetricCategoryKind.Data => Regular("fa-data"),
            MetricCategoryKind.Source => Solid("fa-code-branch"),
            MetricCategoryKind.VersionControl => Solid("fa-code-branch"),
            MetricCategoryKind.Repository => Regular("fa-code-branch"),
            MetricCategoryKind.Dependency => Regular("fa-file-archive"),
            MetricCategoryKind.Framework => Solid("fa-cubes"),
            MetricCategoryKind.Registry => Solid("fa-archive"),
            MetricCategoryKind.Version => Regular("fa-copy"),
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

