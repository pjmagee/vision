using System;
using System.Collections.Generic;
using System.Linq;

namespace Vision.Web.Core
{
    public class DockerAssetExtractor : IAssetExtractor
    {
        public bool Supports(DependencyKind kind) => kind == DependencyKind.Docker;

        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            var lines = asset.Raw.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());

            foreach (var line in lines.Where(line => line.StartsWith("FROM", StringComparison.OrdinalIgnoreCase)))
            {
                string[] segments = line.Split(new[] { "FROM", ":", " " }, StringSplitOptions.RemoveEmptyEntries);
                yield return new Extract(segments[0].Trim(), (segments[segments.Length - 1] == segments[0] ? string.Empty : segments[1]).Trim());
            }
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset)
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
