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

                List<Extract> extracts = new List<Extract>();

                IEnumerable<JToken> dependencies = (
                    package.ContainsKey("dependencies") ? package["dependencies"] : Enumerable.Empty<JToken>())
                    .Concat(
                        package.ContainsKey("devDependencies") ? package["devDependencies"] : Enumerable.Empty<JToken>());

                foreach (string dependency in dependencies.Select((JToken token) => token.ToString()).Distinct())
                {
                    string[] pair = dependency.Split(':');

                    extracts.Add(new Extract(pair[0].Replace("\"", "").Trim(), pair[1].Replace("\"", "").Trim()));
                }

                logger.LogInformation($"Extracted {extracts.Count} dependencies for {asset.Path}");

                return extracts;                
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Could not extract dependencies for {asset.Path}");
            }            

            return Enumerable.Empty<Extract>();
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset)
        {
            try
            {
                JObject package = JObject.Parse(asset.Raw);

                List<Extract> extracts = new List<Extract>();

                if (package.ContainsKey("engines"))
                {
                    var properties = package["engines"].Values<JProperty>();

                    foreach (var prop in properties)
                    {
                        extracts.Add(new Extract(prop.Name, prop.Value.ToString()));
                    }

                    logger.LogInformation($"Extracted {extracts.Count} engines for {asset.Repository.WebUrl} : {asset.Path}");
                }

                return extracts;
            }            
            catch (Exception e)
            {
                logger.LogError(e, $"Could not extract engines for {asset.Repository.WebUrl} : {asset.Path} ");
            }

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
