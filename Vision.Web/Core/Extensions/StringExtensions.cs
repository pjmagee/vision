namespace Vision.Web.Core
{
    using System;
    using System.Linq;

    public static class BooleanExtensions
    {
        public static string ToYesNo(this bool value) => value ? "Yes" : "No";

        public static string ToYesNo(this bool? value) => value.GetValueOrDefault().ToYesNo();
    }
    

    public static class StringExtensions
    {
        public static string ToYesNo(this string value) => string.IsNullOrWhiteSpace(value) ? "No" : "Yes";

        public static DependencyKind GetDependencyKind(this string path)
        {
            return AppHelper.DependencyKinds.Single(kind => path.EndsWith(kind.GetFileExtension(), StringComparison.CurrentCultureIgnoreCase));            
        }

        public static bool IsSupported(this string path)
        {
            if (path.Contains("node_modules")) return false;
            return AppHelper.SupportedExtensions.Any(extension => path.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
