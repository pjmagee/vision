using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Linq;
using System.Xml.Linq;
using Vision.Core;
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

            Assert.Equal("Package.Name", dependencies[0].EcosystemIdentifier);
            Assert.Equal("1.0.0", dependencies[0].EcosystemVersion);
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

            Assert.Equal("Package.Name", dependencies[0].EcosystemIdentifier);
            Assert.Equal("1.0.0", dependencies[0].EcosystemVersion);
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

            Assert.Equal("Package.Name", dependencies[0].EcosystemIdentifier);
            Assert.Equal("1.0.0", dependencies[0].EcosystemVersion);
        }

        [Fact]
        public void ExtractEcosystem()
        {
            // arrange
            var csproj = new XElement("TargetFramework", "netcoreapp2.0").ToString();

            // act
            var ecosystems = sut.ExtractEcoSystem(new Asset { Raw = csproj }).ToList();

            Assert.Equal(".NET", ecosystems[0].EcosystemIdentifier);
            Assert.Equal("netcoreapp2.0", ecosystems[0].EcosystemVersion);
        }

        [Fact]
        public void ExtractMultiTargetFrameworks()
        {
            // arrange
            var csproj = new XElement("TargetFrameworks", "netcoreapp2.0; netstandard2.0").ToString();

            // act
            var ecosystems = sut.ExtractEcoSystem(new Asset { Raw = csproj }).ToList();

            Assert.Equal(".NET", ecosystems[0].EcosystemIdentifier);
            Assert.Equal("netcoreapp2.0", ecosystems[0].EcosystemVersion);

            Assert.Equal(".NET", ecosystems[1].EcosystemIdentifier);
            Assert.Equal("netstandard2.0", ecosystems[1].EcosystemVersion);
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
