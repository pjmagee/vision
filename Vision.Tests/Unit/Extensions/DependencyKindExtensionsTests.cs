namespace Vision.Tests
{
    using Vision.Web.Core;
    using Xunit;

    public class EcosystemKindExtensionsTests
    {
        [Theory]
        [InlineData(EcosystemKind.Npm, AppHelper.NpmFile)]
        [InlineData(EcosystemKind.Docker, AppHelper.DockerFile)]
        [InlineData(EcosystemKind.Gradle, AppHelper.GradleFile)]
        [InlineData(EcosystemKind.PyPi, AppHelper.RequirementsFile)]
        [InlineData(EcosystemKind.RubyGem, AppHelper.RubyGemFile)]
        [InlineData(EcosystemKind.NuGet, AppHelper.NuGetFile)]
        [InlineData(EcosystemKind.Maven, AppHelper.MavenFile)]
        public void GetFileExtensionTests(EcosystemKind kind, string expected) => 
            Assert.Equal(expected, actual: kind.GetFileExtension());
    }
}
