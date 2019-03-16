using System;

namespace Vision.Web.Core
{

    public class Extract
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public Extract(string name, string version)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }
    }
}
