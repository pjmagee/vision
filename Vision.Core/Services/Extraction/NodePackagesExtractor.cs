using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Vision.Core
{
    public class NodePackagesExtractor : IDependencyExtractor
    {
        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {            
            var package = JObject.Parse(asset.Raw);

            foreach (string dependency in package["dependencies"].Concat(package["devDependencies"]).Select((JToken token) => token.ToString()).Distinct())
            {
                string[] pair = dependency.Split(':');

                yield return new Extract
                {
                    Name = pair[0].Replace("\"", "").Trim(),
                    Version = pair[1].Replace("\"", "").Trim()
                };
            }            
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset) => Enumerable.Empty<Extract>();
    }
}
