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
            CiCdKind.Gitlab => "fab fa-gitlab",
            CiCdKind.Jenkins => "fab fa-jenkins",
            CiCdKind.TeamCity => "fab fa-teamcity",
            _ => string.Empty
        };

        public static string GetFontAwesomeClass(this Metric metric) => 
            metric.DependencyKind.HasValue ? 
            metric.DependencyKind.Value.GetFontAwesomeClass() : 
            metric.CategoryKind.GetFontAwesomeClass();

        private static string GetFontAwesomeClass(this MetricCategoryKind kind)
        {
            switch (kind)
            {
                case MetricCategoryKind.Assets: return "far fa-file-code";
                case MetricCategoryKind.CiCds: return "fas fa-cogs";
                case MetricCategoryKind.Data: return "far fa-data";
                case MetricCategoryKind.Sources: return "fas fa-code-branch";
                case MetricCategoryKind.VersionControls: return "fas fa-code-branch";
                case MetricCategoryKind.Repositories: return "far fa-code-branch";
                case MetricCategoryKind.Dependencies: return "far fa-file-archive";
                case MetricCategoryKind.Frameworks: return "fas fa-cubes";
                case MetricCategoryKind.Registries: return "fas fa-archive";
                case MetricCategoryKind.Versions: return "far fa-copy";
                default: return string.Empty;
            }
        }

        private static string GetFontAwesomeClass(this DependencyKind kind)
        {
            switch (kind)
            {
                case DependencyKind.Docker: return "fab fa-docker";
                case DependencyKind.Gradle: return "fab fa-java";
                case DependencyKind.Maven: return "fab fa-java";
                case DependencyKind.Npm: return "fab fa-npm";
                case DependencyKind.NuGet: return "fab fa-microsoft";
                case DependencyKind.PyPi: return "fab fa-python";
                case DependencyKind.RubyGem: return "fas fa-gem";
                default: return string.Empty;
            }
        }
    }
}

