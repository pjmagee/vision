using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Vision.Core
{
    public class CSharpProjectExtractor : IAssetExtractor
    {
        const string PackageReference = "PackageReference";
        const string DotNetCliToolReference = "DotNetCliToolReference";
        const string Reference = "Reference";
        
        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            XDocument document = XDocument.Parse(asset.Raw);

            var sdkPackageReference = document
                .XPathSelectElements("//*[local-name() = '" + PackageReference + "']")
                .Concat(document.XPathSelectElements("//*[local-name() = '" + DotNetCliToolReference + "']"))
                .Select(FromNewReference)
                .ToList();

            // We don't want to pick up on standard library Includes (e.g System or System.Xml) which are in common in .NET Framework / Non .NET Core projects
            // We also don't want to skip any packages that do not contain a hint path, and we want to make sure we don't need to specifically look out for <SpecificVersion> neither.
            // Make it generic enough to find both the <Reference> element but it must have an Attribute of "Include" and contain the value "Version" within it.
            var oldPackageReference = document
                .XPathSelectElements("//*[local-name() = '" + Reference + "']")
                .Where(x => x.HasAttributes && x.FirstAttribute.Name == "Include" && x.FirstAttribute.Value.Contains("Version"))
                .Select(FromOldReference).ToList();

            return sdkPackageReference.Concat(oldPackageReference);
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset)
        {
            XDocument document = XDocument.Parse(asset.Raw);

            IEnumerable<string> targetFramework = (from element in document.Descendants()
                                                   where (element.Name.LocalName == "TargetFramework" || element.Name.LocalName == "TargetFrameworkVersion")
                                                   select element.Value);

            IEnumerable<string> targetFrameworks = (from element in document.Descendants()
                                                    where element.Name.LocalName == "TargetFrameworks"
                                                    let multiple = element.Value
                                                    from framework in multiple.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                                                    select framework);

            return targetFrameworks.Concat(targetFramework).Select(fw => new Extract("Framework", fw));
        }

        private Extract FromNewReference(XElement reference)
        {
            var name = reference?.Attribute("Include")?.Value;
            var version = reference?.Attribute("Version")?.Value ?? reference?.Element("Version")?.Value;

            return new Extract(name?.Trim(), version?.Trim());
        }

        private Extract FromOldReference(XElement reference)
        {                     
            var include = reference.Attribute("Include")?.Value;
            var segments = include.Split(',');

            string name = segments[0].Trim();
            string version = null;

            foreach(string segment in segments)
            {
                var pair = segment.Split('=');

                if (pair[0].Trim() == "Version")
                {
                    version = pair[1].Trim();
                    break;
                }
            }

            return new Extract(name, version);
        }
    }
}
