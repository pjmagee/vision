using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Vision.Core
{
    public class NuGetPackageExtractionService : IExtractionService
    {
        const string PackageReference = "PackageReference";
        const string DotNetCliToolReference = "DotNetCliToolReference";
        const string Reference = "Reference";
        const string Unknown = "Unknown";
        const string Pattern = "\\\\(?<package>[^\\s\\\\.][\\w-\\.]+[^\\d])\\.(?<version>[\\d\\.]+)\\\\";

        static readonly Regex Regex = new Regex(Pattern, RegexOptions.Compiled);

        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            var document = XDocument.Parse(asset.Raw);

            var sdkPackageReference = document
                .XPathSelectElements("//*[local-name() = '" + PackageReference + "']")
                .Concat(document.XPathSelectElements("//*[local-name() = '" + DotNetCliToolReference + "']"))
                .Select(FromNewReference)
                .ToList();

            var oldPackageReference = document
                .XPathSelectElements("//*[local-name() = '" + Reference + "']")
                .Where(x => x.Elements().Any(e => e.Name.LocalName == "HintPath"))
                .Select(FromOldReference).ToList();

            return sdkPackageReference.Concat(oldPackageReference);
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset)
        {
            var document = XDocument.Parse(asset.Raw);

            IEnumerable<string> targetFramework = (from element in document.Descendants()
                                                   where (element.Name.LocalName == "TargetFramework" || element.Name.LocalName == "TargetFrameworkVersion")
                                                   select element.Value);

            IEnumerable<string> targetFrameworks = (from element in document.Descendants()
                                                    where element.Name.LocalName == "TargetFrameworks"
                                                    let multiple = element.Value
                                                    from framework in multiple.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                                                    select framework);

            return targetFrameworks.Concat(targetFramework).Select(fw => new Extract { Version = fw });
        }

        private Extract FromNewReference(XElement reference)
        {
            var package = reference?.Attribute("Include")?.Value;
            var current = reference?.Attribute("Version")?.Value;
            return new Extract { Name = package, Version = current };
        }

        private Extract FromOldReference(XElement reference)
        {
            var hintPath = reference.Elements().First(x => x.Name.LocalName == "HintPath").Value;
            var matches = Regex.Match(hintPath);
            var package = matches.Groups["package"].Value;
            var version = matches.Groups["version"].Value;
            var name = string.IsNullOrEmpty(package) ? hintPath : package;
            var current = string.IsNullOrEmpty(version) ? Unknown : version;

            return new Extract { Name = name, Version = current };
        }
    }
}
