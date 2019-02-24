using System.ComponentModel;

namespace Vision.Shared
{
    public enum TaskScope
    {
        [Description("Version control system")]
        VersionControl,
        [Description("VCS repository")]
        Repository,
        [Description("Asset")]
        Asset,
        [Description("Dependency")]
        Dependency
    }
}
