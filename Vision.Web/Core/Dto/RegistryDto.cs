namespace Vision.Web.Core
{
    using System;

    public class RegistryDto
    {
        public Guid RegistryId { get; set;  }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsPublic { get; set; }
        public DependencyKind Kind { get; set; }
        public int Dependencies { get; set; }
    }
}
