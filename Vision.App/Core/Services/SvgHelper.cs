using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.App
{
    public class SvgHelper
    {
        private readonly HttpClient httpClient;

        private MarkupString BitBucket { get; set; }
        private MarkupString Jenkins { get; set; }
        private MarkupString Github { get; set; }
        private MarkupString Gitlab { get; set; }
        private MarkupString Docker { get; set; }
        private MarkupString Java { get; set; }
        private MarkupString Npm { get; set; }
        private MarkupString NuGet { get; set; }
        private MarkupString Python { get; set; }
        private MarkupString Ruby { get; set; }

        private MarkupString Empty = new MarkupString();               

        public MarkupString GetDependencySvg(DependencyKind kind)
        {
            switch (kind)
            {
                case DependencyKind.Docker: return Docker;
                case DependencyKind.Gradle: return Java;
                case DependencyKind.Maven: return Java;
                case DependencyKind.Npm: return Npm;
                case DependencyKind.NuGet: return NuGet;
                case DependencyKind.PyPi: return Python;
                case DependencyKind.RubyGem: return Ruby;
                default: return Empty;
            }
        }

        public MarkupString GetVersionControlSvg(VersionControlKind kind)
        {
            switch(kind)
            {
                case VersionControlKind.Bitbucket: return BitBucket;
                case VersionControlKind.GitHub: return Github;
                case VersionControlKind.Gitlab: return Gitlab;
                default: return Empty;
            }
        }

        public MarkupString GetCiCdSvg(CiCdKind kind)
        {
            switch(kind)
            {
                case CiCdKind.Gitlab: return Gitlab;
                case CiCdKind.Jenkins: return Jenkins;
                case CiCdKind.TeamCity: return Jenkins;
                default: return Empty;
            }             
        }
               
        public SvgHelper(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task LoadIconsAsync()
        {
            BitBucket = new MarkupString(await httpClient.GetStringAsync("img/bitbucket.svg"));
            Jenkins = new MarkupString(await httpClient.GetStringAsync("img/jenkins.svg"));
            Github = new MarkupString(await httpClient.GetStringAsync("img/github.svg"));
            Gitlab = new MarkupString(await httpClient.GetStringAsync("img/gitlab.svg"));
            Docker = new MarkupString(await httpClient.GetStringAsync("img/docker.svg"));
            Java = new MarkupString(await httpClient.GetStringAsync("img/java.svg"));
            Npm = new MarkupString(await httpClient.GetStringAsync("img/npm.svg"));
            NuGet = new MarkupString(await httpClient.GetStringAsync("img/nuget.svg"));
            Python = new MarkupString(await httpClient.GetStringAsync("img/python.svg"));
            Ruby = new MarkupString(await httpClient.GetStringAsync("img/ruby.svg"));
        }
    }
}


