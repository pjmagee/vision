using System;
using System.Linq;
using System.Xml.Linq;
using Vision.Core;
using Xunit;

namespace Vision.Tests
{

    public class NuGetExtractorTests: IClassFixture<NuGetPackageExtractor>
    {
        private readonly NuGetPackageExtractor extractor;

        public NuGetExtractorTests(NuGetPackageExtractor extractor)
        {
            this.extractor = extractor;
        }

        [Fact]
        public void ExtractPackageReferenceWithVersionAttribute()
        {
            var csproj = new XElement("ItemGroup",
                new XElement("PackageReference",
                    new XAttribute("Include", "Package.Name"),
                    new XAttribute("Version", "1.0.0"))).ToString();

            var dependencies = extractor.ExtractDependencies(new Asset { Id = Guid.NewGuid(), Raw = csproj }).ToList();

            Assert.Equal("Package.Name", dependencies[0].Name);
            Assert.Equal("1.0.0", dependencies[0].Version);
        }

        [Fact]
        public void ExtractPackageReferenceWithVersionElement()
        {
            var csproj = new XElement("ItemGroup",
                    new XElement("PackageReference",
                        new XAttribute("Include", "Package.Name"),
                        new XElement("Version", "1.0.0"))).ToString();

            var dependencies = extractor.ExtractDependencies(new Asset { Id = Guid.NewGuid(), Raw = csproj }).ToList();

            Assert.Equal("Package.Name", dependencies[0].Name);
            Assert.Equal("1.0.0", dependencies[0].Version);
        }

        [Fact]
        public void ExtractPackageReferenceWithReferenceElement()
        {
            var csproj = new XElement("ItemGroup",
                    new XElement("Reference",
                        new XAttribute("Include", "Package.Name, Version=1.0.0, Culture=neutral, PublicKeyToken=" + Guid.NewGuid()),
                        new XElement("SpecificVersion", "False"))).ToString();

            var dependencies = extractor.ExtractDependencies(new Asset { Id = Guid.NewGuid(), Raw = csproj }).ToList();

            Assert.Equal("Package.Name", dependencies[0].Name);
            Assert.Equal("1.0.0", dependencies[0].Version);
        }

    }
}
