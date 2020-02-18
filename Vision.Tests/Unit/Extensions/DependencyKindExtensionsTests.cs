using Vision.Core;
using Xunit;

namespace Vision.Tests
{
    public class EcosystemKindExtensionsTests
    {
        [Theory]
        [InlineData(EcosystemKind.Npm, Constants.NpmFile)]
        [InlineData(EcosystemKind.Docker, Constants.DockerFile)]
        [InlineData(EcosystemKind.Gradle, Constants.GradleFile)]
        [InlineData(EcosystemKind.PyPi, Constants.RequirementsFile)]
        [InlineData(EcosystemKind.RubyGem, Constants.RubyGemFile)]
        [InlineData(EcosystemKind.NuGet, Constants.NuGetFile)]
        [InlineData(EcosystemKind.Maven, Constants.MavenFile)]
        public void GetFileExtensionTests(EcosystemKind kind, string expected) => 
            Assert.Equal(expected, actual: kind.GetFileExtension());
    }
}
