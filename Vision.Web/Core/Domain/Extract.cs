using System;

namespace Vision.Web.Core
{
    public class Extract
    {
        public string RuntimeIdentifier { get; }
        public string RuntimeVersion { get; }

        public Extract(string name, string version)
        {
            RuntimeIdentifier = name ?? throw new ArgumentNullException(nameof(name));
            RuntimeVersion = version ?? throw new ArgumentNullException(nameof(version));
        }
    }
}
