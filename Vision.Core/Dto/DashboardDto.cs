namespace Vision.Core
{
    public class DashboardDto
    {
        public int Assets { get; set; }
        public int Dependencies { get; set; }
        public int DockerDependencies { get; set; }
        public int NPMDependencies { get; set; }
        public int MavenDependencies { get; set; }
        public int DotNetDependencies { get; set; }
    }
}
