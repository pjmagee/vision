using System;
using System.Linq;

namespace Vision.Core
{

    public static class StringExtensions
    {
        public static string ToYesNo(this string value) => string.IsNullOrWhiteSpace(value) ? "No" : "Yes";

        public static EcosystemKind GetEcosystemKind(this string path)
        {
            return Constants.EcosystemKinds.Single(kind => path.EndsWith(kind.GetFileExtension(), StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool IsSupported(this string path)
        {
            if (path.Contains("node_modules")) return false;
            return Constants.SupportedExtensions.Any(extension => path.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
