using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vision.Web.Core
{
    public class PyPiAssetExtractor : IAssetExtractor
    {
        private readonly ILogger<PyPiAssetExtractor> logger;

        public EcosystemKind Kind { get; } = EcosystemKind.PyPi;

        public PyPiAssetExtractor(ILogger<PyPiAssetExtractor> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            var extracts = asset.Raw.Split(Environment.NewLine).Select(line => line.Trim()) // trim any spaces first
                .Where(line => !line.StartsWith(".") && !line.StartsWith("/") && !line.StartsWith("#") && !line.StartsWith("-r") && !line.StartsWith("[-e]") && !line.Contains("http://") && !line.Contains("https://")) // only extract what we can
                .Select(line => line.IndexOf('#') > -1 ? line.Substring(0, line.IndexOf('#')) : line) // cut out any comments for easier extraction
                .Select(line => line.Split(new[] { "==", ">=", "<=", "===", "!=", ">", "<", "~=", "#" }, StringSplitOptions.RemoveEmptyEntries));

            return extracts.Select(extract => new Extract(extract.First()?.Trim(), extract.Last()?.Trim()));
        }

        public IEnumerable<Extract> ExtractEcoSystem(Asset asset)
        {
            // https://pip.pypa.io/en/stable/reference/pip_install/#requirement-specifiers
            // SomeProject ==5.4 ; python_version < '2.7'
            // SomeProject; sys_platform == 'win32'

            yield break; // Python Version ??
        }

        public string ExtractPublishName(Asset asset)
        {
            throw new NotImplementedException();
        }
    }
}
