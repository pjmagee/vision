using System;
using System.Linq;

namespace Vision.Web.Core
{

    public static class StringExtensions
    {
        public static string ToYesNo(this string value) => string.IsNullOrWhiteSpace(value) ? "No" : "Yes";

        public static EcosystemKind GetEcosystemKind(this string path)
        {
            return AppHelper.EcosystemKinds.Single(kind => path.EndsWith(kind.GetFileExtension(), StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool IsSupported(this string path)
        {
            if (path.Contains("node_modules")) return false;
            return AppHelper.SupportedExtensions.Any(extension => path.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
