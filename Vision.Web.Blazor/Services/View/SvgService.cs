using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vision.Core;

namespace Vision.Web.Blazor.Services
{
    public class SvgService
    {
        private readonly Dictionary<string, MarkupString> svgs = new Dictionary<string, MarkupString>();
        private readonly MarkupString Empty = new MarkupString();

        public MarkupString GetSvg(EcosystemKind kind) => kind switch
        {
            EcosystemKind.Docker => svgs["docker"],
            EcosystemKind.Gradle => svgs["gradle"],
            EcosystemKind.Maven => svgs["java"],
            EcosystemKind.Npm => svgs["npm"],
            EcosystemKind.NuGet => svgs["nuget"],
            EcosystemKind.PyPi => svgs["python"],
            EcosystemKind.RubyGem => svgs["ruby"],
            _ => Empty
        };

        public MarkupString GetSvg(VcsKind kind) => kind switch
        {
            VcsKind.Bitbucket => svgs["bitbucket"],
            VcsKind.GitHub => svgs["github"],
            VcsKind.Gitlab => svgs["gitlab"],
            _ => Empty
        };

        public MarkupString GetSvg(CiCdKind kind) => kind switch
        {
            CiCdKind.Gitlab => svgs["gitlab"],
            CiCdKind.Jenkins => svgs["jenkins"],
            CiCdKind.TeamCity => svgs["teamcity"],
            _ => Empty
        };

        public SvgService(IWebHostEnvironment environment)
        {
            foreach (IFileInfo svg in environment.WebRootFileProvider.GetDirectoryContents("img").Where(f => Path.GetExtension(f.PhysicalPath).Equals(".svg")))
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
