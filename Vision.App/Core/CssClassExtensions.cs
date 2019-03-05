using Vision.Shared;

namespace Vision.App
{
    public static class CssClassExtensions
    {
        public static string GetIconClass(this CiCdKind kind) => $"icon {kind.ToString()}";
        public static string GetIconClass(this MetricCategoryKind kind) => $"icon {kind.ToString()}";
        public static string GetIconClass(this DependencyKind kind) => $"icon {kind.ToString()}";
        public static string GetIconClass(this MetricAlertKind kind) => $"icon {kind.ToString()}";
        public static string GetIconClass(this VersionControlKind kind) => $"icon {kind.ToString()}";


        public static string GetFontAwesomeClass(this CiCdKind kind)
        {
            switch(kind)
            {
                case CiCdKind.Gitlab: return "fab fa-gitlab";
                case CiCdKind.Jenkins: return "fab fa-jenkins";
                case CiCdKind.TeamCity: return "fab fa-teamcity";
                default: return string.Empty;
            }
        }

        public static string GetFontAwesomeClass(this MetricCategoryKind kind)
        {
            switch(kind)
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

        public static string GetFontAwesomeClass(this DependencyKind kind)
        {
            switch(kind)
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
               

        public static string GetMetricColour(this MetricAlertKind kind)
        {
            switch(kind)
            {
                case MetricAlertKind.Standard: return "primary";
                case MetricAlertKind.Good: return "success";
                case MetricAlertKind.Warning: return "warning";                
                case MetricAlertKind.Bad: return "danger";
                default: return string.Empty;
            }
        }

        public static string GetFontAwesomeClass(this Metric metric)
        {
            if (metric.DependencyKind.HasValue)
            {
                return metric.DependencyKind.Value.GetFontAwesomeClass();
            }

            return metric.CategoryKind.GetFontAwesomeClass();
        }
    }
}

