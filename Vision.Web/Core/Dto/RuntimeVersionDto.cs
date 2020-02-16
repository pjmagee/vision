using System;

namespace Vision.Web.Core
{
    public class RuntimeVersionDto
    {
        public string Version { get; set; }
        public Guid RuntimeVersionId { get; set; }
        public Guid RuntimeId { get; set; }
        public int Assets { get; set; }
    }
}
