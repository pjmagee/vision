using System.ComponentModel;

namespace Vision.Shared
{
    public enum MetricCategoryKind
    {        
        [Description("Sources")]
        Sources,
        [Description("Data")]
        Data,
        [Description("Version control")]
        VersionControls,
        [Description("Repositories")]
        Repositories,
        [Description("Assets")]
        Assets,
        [Description("Dependencies")]
        Dependencies,
        [Description("Frameworks")]
        Frameworks,
        [Description("Dependency registries")]
        Registries,
        [Description("Dependency versions")]
        Versions,
        [Description("CICD")]
        CiCds
    }
}
