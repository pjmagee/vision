namespace Vision.Tests
{
    using System;
    using Vision.Web.Core;
    using Xunit;

    public class StringExtensionTests
    {
        [Fact]
        public void ExtensionsShouldBeSupported()
        {
            Assert.All(AppHelper.SupportedExtensions, item => Assert.True(item.IsSupported()));                
        }

        [Fact]
        public void RandomExtensionShouldNotReturnDependencyKind()
        {
            Assert.Throws<InvalidOperationException>(() => ".unsupported".GetDependencyKind());
        }
    }
}
