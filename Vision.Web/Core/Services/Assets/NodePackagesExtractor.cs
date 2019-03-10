using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Vision.Web.Core
{
    public class NPMAssetExtractor : IAssetExtractor
    {
        public bool Supports(DependencyKind kind) => kind == DependencyKind.Npm;

        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            JObject package = JObject.Parse(asset.Raw);

            foreach (string dependency in package["dependencies"].Concat(package["devDependencies"]).Select((JToken token) => token.ToString()).Distinct())
            {
                string[] pair = dependency.Split(':');

                yield return new Extract(pair[0].Replace("\"", "").Trim(), pair[1].Replace("\"", "").Trim());
            }            
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset) => Enumerable.Empty<Extract>();
    }
}
