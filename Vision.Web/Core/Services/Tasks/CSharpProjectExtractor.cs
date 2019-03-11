﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Vision.Web.Core
{
    public class NuGetAssetExtractor : IAssetExtractor
    {
        const string PackageReference = "PackageReference";
        const string DotNetCliToolReference = "DotNetCliToolReference";
        const string Reference = "Reference";
        private readonly ILogger<NuGetAssetExtractor> logger;

        public bool Supports(DependencyKind kind) => kind == DependencyKind.NuGet;

        public NuGetAssetExtractor(ILogger<NuGetAssetExtractor> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            try
            {
                // We cannot use XDocument.Parse because of bom issues with SOME of our assets.
                // If we force it to get the encoding with UTF8 and let the XML Text Reader handle it, it fixes most of those issues :-)
                using (var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(asset.Raw)))
                {
                    using (var xmlReader = new XmlTextReader(xmlStream))
                    {
                        XDocument document = XDocument.Load(xmlReader);

                        List<Extract> sdkPackageReference = document
                            .XPathSelectElements("//*[local-name() = '" + PackageReference + "']")
                            .Concat(document.XPathSelectElements("//*[local-name() = '" + DotNetCliToolReference + "']"))
                            .Select(FromNewReference)
                            .ToList();

                        // We don't want to pick up on standard library Includes (e.g System or System.Xml) which are in common in .NET Framework / Non .NET Core projects
                        // We also don't want to skip any packages that do not contain a hint path, and we want to make sure we don't need to specifically look out for <SpecificVersion> neither.
                        // Make it generic enough to find both the <Reference> element but it must have an Attribute of "Include" and contain the value "Version" within it.
                        List<Extract> oldPackageReference = document
                            .XPathSelectElements("//*[local-name() = '" + Reference + "']")
                            .Where(x => x.HasAttributes && x.FirstAttribute.Name == "Include" && x.FirstAttribute.Value.Contains("Version"))
                            .Select(FromOldReference).ToList();

                        var results = sdkPackageReference.Concat(oldPackageReference).ToList();

                        logger.LogInformation($"Extracted {results.Count} frameworks for asset {asset.Path}");

                        return results;
                    }
                }
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Could not extract dependencies for asset {asset.Path}");
            }

            return Enumerable.Empty<Extract>();            
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset)
        {
            try
            {
                // We cannot use XDocument.Parse because of bom issues with SOME of our assets.
                // If we force it to get the encoding with UTF8 and let the XML Text Reader handle it, it fixes most of those issues :-)

                using (var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(asset.Raw)))
                {
                    using (var xmlReader = new XmlTextReader(xmlStream))
                    {
                        XDocument document = XDocument.Load(xmlReader);

                        IEnumerable<string> targetFramework = (from element in document.Descendants()
                                                               where (element.Name.LocalName == "TargetFramework" || element.Name.LocalName == "TargetFrameworkVersion")
                                                               select element.Value);

                        IEnumerable<string> targetFrameworks = (from element in document.Descendants()
                                                                where element.Name.LocalName == "TargetFrameworks"
                                                                let multiple = element.Value
                                                                from framework in multiple.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                                                                select framework);


                        var results = targetFrameworks.Concat(targetFramework).Select(fw => new Extract("Framework", fw)).ToList();

                        logger.LogInformation($"Extracted {results.Count} frameworks for asset {asset.Path}");

                        return results;


                    }
                }
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Could not extract frameworks for asset {asset.Path}");
            }

            return Enumerable.Empty<Extract>();
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
