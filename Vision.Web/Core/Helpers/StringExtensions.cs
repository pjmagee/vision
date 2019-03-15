namespace Vision.Web.Core
{
    using System;
    using System.Linq;

    public static class StringExtensions
    {
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
