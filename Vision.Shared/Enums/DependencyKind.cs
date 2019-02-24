using System.ComponentModel;

namespace Vision.Shared
{
    public enum DependencyKind
    {
        [Description("Python Package Index (eg requirements.txt)")]
        PyPi,
        [Description("Node Package Manager (eg packages.json)")]
        Npm,
        [Description("Packages for .NET (eg .csproj, packages.config)")]
        NuGet,
        [Description("Docker Images (Dockerfile)")]
        Docker,
        [Description("JVM ecosystem (pom.xml)")]
        Maven,
        [Description("JVM ecosystem (build.gradle)")]
        Gradle,
        [Description("Ruby Gems (eg GemFile)")]
        RubyGem
    }
}
