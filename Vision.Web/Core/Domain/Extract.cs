using System;

namespace Vision.Web.Core
{
    public class Extract
    {
        public string Name { get; }
        public string Version { get; }

        public Extract(string name, string version)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }
    }
}
