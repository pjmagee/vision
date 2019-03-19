namespace Vision.Web.Core
{
    using System;
    public class FrameworkDto
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public Guid FrameworkId { get; set; }
        public int Assets { get; set; }
    }
}
