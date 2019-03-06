using Vision.Core;
using Vision.Shared;
using Xunit;

namespace Vision.Tests
{

    public class AppHelperTests
    {
        [Fact]
        public void DependencyKindForCsProjIsNuGet()
        {
            // arrange
            var asset = new Asset { Path = ".csproj" };

            // act
            var kind = AppHelper.GetDependencyKind(asset);

            // asset

            Assert.Equal(DependencyKind.NuGet, kind);
        }

        [Fact]
        public void DependencyKindForPomFileIsMaven()
        {
            // arrange
            var asset = new Asset { Path = ".pom.xml" };

            // act
            var kind = AppHelper.GetDependencyKind(asset);

            // asset
            Assert.Equal(DependencyKind.Maven, kind);
        }

        [Fact]
        public void DependencyKindForRequirementsTxtIsPyPi()
        {
            // arrange
            var asset = new Asset { Path = "requirements.txt" };

            // act
            var kind = AppHelper.GetDependencyKind(asset);

            // asset
            Assert.Equal(DependencyKind.PyPi, kind);
        }
    }
}
