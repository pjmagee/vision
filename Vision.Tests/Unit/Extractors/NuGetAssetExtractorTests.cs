using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Linq;
using System.Xml.Linq;
using Vision.Web.Core;
using Xunit;

namespace Vision.Tests
{
    public class NuGetAssetExtractorTests
    {
        private readonly NuGetAssetExtractor sut;

        public NuGetAssetExtractorTests()
        {
            this.sut = new NuGetAssetExtractor(Substitute.For<ILogger<NuGetAssetExtractor>>());
        }

        [Fact]
        public void ExtractPackageReferenceWithVersionAttribute()
        {
            // arrange
            var csproj = new XElement("ItemGroup",
                new XElement("PackageReference",
                    new XAttribute("Include", "Package.Name"),
                    new XAttribute("Version", "1.0.0"))).ToString();

            // act
            var dependencies = sut.ExtractDependencies(new Asset { Raw = csproj }).ToList();

            Assert.Equal("Package.Name", dependencies[0].RuntimeIdentifier);
            Assert.Equal("1.0.0", dependencies[0].RuntimeVersion);
        }

        [Fact]
        public void ExtractPackageReferenceWithVersionElement()
        {
            // arrange
            var csproj = new XElement("ItemGroup",
                    new XElement("PackageReference",
                        new XAttribute("Include", "Package.Name"),
                        new XElement("Version", "1.0.0"))).ToString();

            // act
            var dependencies = sut.ExtractDependencies(new Asset { Raw = csproj }).ToList();

            Assert.Equal("Package.Name", dependencies[0].RuntimeIdentifier);
            Assert.Equal("1.0.0", dependencies[0].RuntimeVersion);
        }

        [Fact]
        public void ExtractPackageReferenceWithReferenceElement()
        {
            // arrange
            var csproj = new XElement("ItemGroup",
                    new XElement("Reference",
                        new XAttribute("Include", "Package.Name, Version=1.0.0, Culture=neutral, PublicKeyToken=" + Guid.NewGuid()),
                        new XElement("SpecificVersion", "False"))).ToString();

            // act
            var dependencies = sut.ExtractDependencies(new Asset { Raw = csproj }).ToList();

            Assert.Equal("Package.Name", dependencies[0].RuntimeIdentifier);
            Assert.Equal("1.0.0", dependencies[0].RuntimeVersion);
        }

        [Fact]
        public void ExtractSingleFramework()
        {
            // arrange
            var csproj = new XElement("TargetFramework", "netcoreapp2.0").ToString();

            // act
            var frameworks = sut.ExtractRuntimes(new Asset { Raw = csproj }).ToList();

            Assert.Equal(".NET", frameworks[0].RuntimeIdentifier);
            Assert.Equal("netcoreapp2.0", frameworks[0].RuntimeVersion);
        }

        [Fact]
        public void ExtractMultiTargetFrameworks()
        {
            // arrange
            var csproj = new XElement("TargetFrameworks", "netcoreapp2.0; netstandard2.0").ToString();

            // act
            var frameworks = sut.ExtractRuntimes(new Asset { Raw = csproj }).ToList();

            Assert.Equal(".NET", frameworks[0].RuntimeIdentifier);
            Assert.Equal("netcoreapp2.0", frameworks[0].RuntimeVersion);

            Assert.Equal(".NET", frameworks[1].RuntimeIdentifier);
            Assert.Equal("netstandard2.0", frameworks[1].RuntimeVersion);
        }

        [Fact]
        public void ExtractsPublishNameUsingPackageIdWhenPresent()
        {
            // arrange
            var csproj = new XElement("PackageId", "Vision").ToString();

            // act
            var name = sut.ExtractPublishName(new Asset { Raw = csproj }).ToList();

            Assert.Equal("Vision", name);
        }

        [Fact]
        public void ExtractsPublishNameUsingFileNameWhenPackageIdNotPresent()
        {
            // arrange
            var csproj = new XElement("Project", new XElement("With", new XElement("No", new XElement("Publish", new XElement("Name"))))).ToString();
            // act
            var name = sut.ExtractPublishName(new Asset { Raw = csproj, Path = "Vision.Example.csproj" }).ToList();

            Assert.Equal("Vision.Example", name);
        }
    }
}
