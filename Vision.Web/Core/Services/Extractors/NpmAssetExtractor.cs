using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Vision.Web.Core
{
    public class NpmAssetExtractor : IAssetExtractor
    {
        private readonly ILogger<NpmAssetExtractor> logger;

        public bool Supports(DependencyKind kind) => kind == DependencyKind.Npm;

        public NpmAssetExtractor(ILogger<NpmAssetExtractor> logger)
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

        public IEnumerable<Extract> ExtractFrameworks(Asset asset)
        {
            // ¯\_(ツ)_/¯ What is a 'Framework' for the Node world?
            // TODO: extract example 'engines' (I guess this is the closest to frameworks/runtime for node?)
            // TODO: { "engines" : { "node" : ">=0.10.3 <0.12" } }
            // TODO: { "engines" : { "npm" : "~1.0.20" } }
            return Enumerable.Empty<Extract>();
        }

        public string ExtractPublishName(Asset asset)
        {
            try
            {
                return JObject.Parse(asset.Raw).GetValue("name").ToString();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Could not extract name from {asset.Path}");
            }

            return string.Empty;
        }
    }
}
