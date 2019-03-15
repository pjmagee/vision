namespace Vision.Tests
{
    using Vision.Web.Core;
    using Xunit;

    public class DependencyKindExtensionsTests
    {
        [Fact]
        public void NpmIsNodePackageFile()
        {
            // arrange
            var kind = DependencyKind.Npm;

            // act
            var expected = kind.GetFileExtension();

            Assert.Equal(AppHelper.NodePackageFile, expected);
        }

        [Fact]
        public void MavenIsPomFile()
        {
            // arrange
            var kind = DependencyKind.Maven;

            // act
            var expected = kind.GetFileExtension();

            Assert.Equal(AppHelper.MavenPomFile, expected);
        }

        [Fact]
        public void DockerIsDockerfile()
        {
            // arrange
            var kind = DependencyKind.Docker;

            // act
            var expected = kind.GetFileExtension();

            Assert.Equal(AppHelper.DockerFile, expected);
        }

        [Fact]
        public void PyPiIsRequirementsFile()
        {
            // arrange
            var kind = DependencyKind.PyPi;

            // act
            var expected = kind.GetFileExtension();

            Assert.Equal(AppHelper.PythonRequirementsFile, expected);
        }

        [Fact]
        public void RubyGemIsGemsFile()
        {
            // arrange
            var kind = DependencyKind.RubyGem;

            // act
            var expected = kind.GetFileExtension();

            Assert.Equal(AppHelper.RubyGemFile, expected);
        }
    }
}
