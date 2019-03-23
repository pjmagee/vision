namespace Vision.Web.Core
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.FileProviders;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SvgService
    {
        private readonly Dictionary<string, MarkupString> svgs = new Dictionary<string, MarkupString>();
        private readonly MarkupString Empty = new MarkupString();           

        public MarkupString GetSvg(DependencyKind kind)
        {
            return kind switch
            {
                DependencyKind.Docker => svgs["docker"],
                DependencyKind.Gradle => svgs["gradle"],
                DependencyKind.Maven => svgs["java"],
                DependencyKind.Npm => svgs["npm"],
                DependencyKind.NuGet => svgs["nuget"],
                DependencyKind.PyPi => svgs["python"],
                DependencyKind.RubyGem => svgs["ruby"],
                _ => Empty
            };
        }

        public MarkupString GetSvg(VersionControlKind kind)
        {
            return kind switch
            {
                VersionControlKind.Bitbucket => svgs["bitbucket"],
                VersionControlKind.GitHub => svgs["github"],
                VersionControlKind.Gitlab => svgs["gitlab"],
                _ => Empty
            };
        }

        public MarkupString GetSvg(CiCdKind kind)
        {
            return kind switch
            {
                CiCdKind.Gitlab => svgs["gitlab"],
                CiCdKind.Jenkins => svgs["jenkins"],
                CiCdKind.TeamCity => svgs["teamcity"],
                _ => Empty
            };
        }
               
        public SvgService(IWebHostEnvironment environment)
        {
            foreach(IFileInfo svg in environment.WebRootFileProvider.GetDirectoryContents("img").Where(f => f.PhysicalPath.EndsWith("svg")))
            {
                using (Stream stream = svg.CreateReadStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        svgs[Path.GetFileNameWithoutExtension(svg.Name)] = new MarkupString(reader.ReadToEnd());
                    }
                }
            }
        }
    }
}
