namespace Vision.Web.Core
{
    public class Registry : Entity
    {
        public string Endpoint { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsPublic { get; set; }
        public DependencyKind Kind { get; set; }        
        public string ApiKey { get; set; }        
        public string Username { get; set; }
        public string Password { get; set; }        
    }
}
