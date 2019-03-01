using Vision.Shared;

namespace Vision.App
{
    public static class IconHelper
    {
        public static string GetCiCdIcon(this CiCdDto cicd)
        {
            switch(cicd.Kind)
            {
                case CiCdKind.Gitlab: return "fab fa-gitlab";
                case CiCdKind.Jenkins: return "fab fa-jenkins";
                case CiCdKind.TeamCity: return "fab fa-teamcity";
                default: return string.Empty;
            }
        }

        public static string GetMetricKindIcon(this MetricCategoryKind kind)
        {
            switch(kind)
            {
                case MetricCategoryKind.Assets: return "far fa-file-code";
                case MetricCategoryKind.CiCds: return "fas fa-cogs";
                case MetricCategoryKind.Data: return "far fa-data";
                case MetricCategoryKind.Sources: return "fas fa-code-branch";
                case MetricCategoryKind.VersionControls: return "far fa-code-branch";
                case MetricCategoryKind.Repositories: return "far fa-folder";
                case MetricCategoryKind.Dependencies: return "far fa-file-archive";
                case MetricCategoryKind.Frameworks: return "fas fa-cubes";
                case MetricCategoryKind.Registries: return "fas fa-archive";
                case MetricCategoryKind.Versions: return "far fa-copy";
                default: return string.Empty;
            }
        }

        public static string GetDependencyKindIcon(this DependencyKind kind)
        {
            switch(kind)
            {
                case DependencyKind.Docker: return "fab fa-docker";
                case DependencyKind.Gradle: return "fab fa-java";
                case DependencyKind.Maven: return "fab fa-java";
                case DependencyKind.Npm: return "fab fa-npm";
                case DependencyKind.NuGet: return "fab fa-microsoft";
                case DependencyKind.PyPi: return "fab fa-python";
                case DependencyKind.RubyGem: return "fab fa-gem";
                default: return string.Empty;
            }
        }

        public static string GetMetricColour(this Metric metric)
        {
            switch(metric.Kind)
            {
                case MetricKind.Standard: return "primary";
                case MetricKind.Good: return "success";
                case MetricKind.Warning: return "warning";                
                case MetricKind.Bad: return "danger";
                default: return string.Empty;
            }
        }

        public static string GetFontAwesomeIcon(this Metric metric)
        {
            if (metric.DependencyKind.HasValue)
            {
                return metric.DependencyKind.Value.GetDependencyKindIcon();
            }

            return metric.CategoryKind.GetMetricKindIcon();
        }
    }
}

