using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Vision.Web.Core
{
    public class NPMAssetExtractor : IAssetExtractor
    {
        private readonly ILogger<NPMAssetExtractor> logger;

        public bool Supports(DependencyKind kind) => kind == DependencyKind.Npm;

        public NPMAssetExtractor(ILogger<NPMAssetExtractor> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            try
            {
                JObject package = JObject.Parse(asset.Raw);

                List<Extract> results = new List<Extract>();

                foreach (string dependency in package["dependencies"].Concat(package["devDependencies"]).Select((JToken token) => token.ToString()).Distinct())
                {
                    string[] pair = dependency.Split(':');

                    results.Add(new Extract(pair[0].Replace("\"", "").Trim(), pair[1].Replace("\"", "").Trim()));
                }

                logger.LogInformation($"Extracted {results.Count} dependencies for {asset.Path}");

                return results;                
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Could not extract dependencies for {asset.Path}");
            }            

            return Enumerable.Empty<Extract>();
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset) => Enumerable.Empty<Extract>();
    }
}
