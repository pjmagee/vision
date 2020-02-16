namespace Vision.Web.Core
{
    using System;

    public class DependencyDto
    {
        public Guid DependencyId { get; set; }
        public string Name { get; set; }
        public string RepositoryUrl { get; set; }
        public EcosystemKind Kind { get; set; }                
        public int Assets { get; set; }
        public int Versions { get; set; }
    }
}
