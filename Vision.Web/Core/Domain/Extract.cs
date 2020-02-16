using System;

namespace Vision.Web.Core
{
    public class Extract
    {
        public string EcosystemIdentifier { get; }
        public string EcosystemVersion { get; }

        public Extract(string name, string version)
        {
            EcosystemIdentifier = name ?? throw new ArgumentNullException(nameof(name));
            EcosystemVersion = version ?? throw new ArgumentNullException(nameof(version));
        }
    }
}
