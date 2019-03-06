namespace Vision.Core
{
    public class Extract
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public Extract(string name, string version)
        {
            Name = name;
            Version = version;
        }
    }
}
