using Vision.Web.Core;
using Xunit;

namespace Vision.Tests
{

    public class AssetExtensionsTests
    {
        [Fact]
        public void DependencyKindForCsProjIsNuGet()
        {
            // arrange
            var asset = new Asset { Path = ".csproj" };

            // act
            var kind = asset.GetDependencyKind();

            // assert

            Assert.Equal(DependencyKind.NuGet, kind);
        }

        [Fact]
        public void DependencyKindForPomFileIsMaven()
        {
            // arrange
            var asset = new Asset { Path = ".pom.xml" };

            // act
            var kind = asset.GetDependencyKind();

            // assert
            Assert.Equal(DependencyKind.Maven, kind);
        }

        [Fact]
        public void DependencyKindForRequirementsTxtIsPyPi()
        {
            // arrange
            var asset = new Asset { Path = "requirements.txt" };

            // act
            var kind = asset.GetDependencyKind();

            // assert
            Assert.Equal(DependencyKind.PyPi, kind);
        }
    }
}
