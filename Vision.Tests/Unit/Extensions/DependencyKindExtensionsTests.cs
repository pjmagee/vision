namespace Vision.Tests
{
    using Vision.Web.Core;
    using Xunit;

    public class DependencyKindExtensionsTests
    {
        [Theory]
        [InlineData(DependencyKind.Npm, AppHelper.NpmFile)]
        [InlineData(DependencyKind.Docker, AppHelper.DockerFile)]
        [InlineData(DependencyKind.Gradle, AppHelper.GradleFile)]
        [InlineData(DependencyKind.PyPi, AppHelper.RequirementsFile)]
        [InlineData(DependencyKind.RubyGem, AppHelper.RubyGemFile)]
        [InlineData(DependencyKind.NuGet, AppHelper.NuGetFile)]
        [InlineData(DependencyKind.Maven, AppHelper.MavenFile)]
        public void GetFileExtensionTests(DependencyKind kind, string expected) => 
            Assert.Equal(expected, actual: kind.GetFileExtension());
    }
}
