using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vision.Core
{
    public class DockerAssetExtractor : IAssetExtractor
    {
        private readonly ILogger<DockerAssetExtractor> logger;

        public EcosystemKind Kind { get; } = EcosystemKind.Docker;

        public DockerAssetExtractor(ILogger<DockerAssetExtractor> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            var lines = asset.Raw.Split(Environment.NewLine).Select(x => x.Trim());
            var ignores = new List<string>();

            foreach (var line in lines.Where(line => line.StartsWith("FROM", StringComparison.OrdinalIgnoreCase)))
            {
                string[] segments = line.Split(new[] { "from ", "FROM ", ":", " AS ", " as " }, StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length >= 3)
                {
                    ignores.Add(segments[2]);
                }

                if (ignores.Contains(segments[0].Trim()))
                {
                    continue;
                }

                yield return new Extract(segments[0].Trim(), (segments[segments.Length - 1] == segments[0] ? string.Empty : segments[1]).Trim());
            }
        }

        public IEnumerable<Extract> ExtractEcoSystem(Asset asset)
        {
            // ¯\_(ツ)_/¯ Erm...
            // Do we....list previous docker images? No no no.
            yield break;
        }

        public string ExtractPublishName(Asset asset)
        {
            // TODO: Ensure ALL Dockerfiles contain the LABEL keyword with 'Name' set to the name we publish the image as.
            // TODO: read 'LABEL' line in Dockerfile with 'Name' property
            throw new NotImplementedException();
        }
    }
}
