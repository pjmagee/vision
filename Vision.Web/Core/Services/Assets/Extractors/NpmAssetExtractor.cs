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
            List<Extract> extracts = new List<Extract>();

            try
            {
                JObject package = JObject.Parse(asset.Raw);
                IEnumerable<JToken> dependencies = package.ContainsKey("dependencies") ? package["dependencies"] : Enumerable.Empty<JToken>();
                IEnumerable<JToken> devDependencies = package.ContainsKey("devDependencies") ? package["devDependencies"] : Enumerable.Empty<JToken>();
                IEnumerable<JToken> all = dependencies.Concat(devDependencies);

                foreach (string dependency in all.Select((JToken token) => token.ToString()).Distinct())
                {
                    string[] pair = dependency.Split(':');
                    extracts.Add(new Extract(pair[0].Replace("\"", "").Trim(), pair[1].Replace("\"", "").Trim()));
                }

                logger.LogTrace($"Extracted {extracts.Count} dependencies for {asset.Path}");

                return extracts;                
            }
            catch(Exception e)
            {
                logger.LogTrace(e, $"Could not extract dependencies for {asset.Path}");
            }            

            return Enumerable.Empty<Extract>();
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset)
        {
            List<Extract> extracts = new List<Extract>();

            try
            {
                JObject package = JObject.Parse(asset.Raw);                

                if (package.ContainsKey("engines"))
                {
                    IEnumerable<JProperty> properties = package["engines"].Values<JProperty>();

                    foreach (var prop in properties)
                    {
                        extracts.Add(new Extract(prop.Name, prop.Value.ToString()));
                    }

                    logger.LogTrace($"Extracted {extracts.Count} engines for {asset.Repository.WebUrl} : {asset.Path}");
                }                
            }            
            catch (Exception e)
            {
                logger.LogTrace(e, $"Could not extract engines for {asset.Repository.WebUrl} : {asset.Path} ");
            }

            foreach (Extract extract in extracts)
            {
                yield return extract;
            }   
        }

        public string ExtractPublishName(Asset asset)
        {
            try
            {
                return JObject.Parse(asset.Raw).GetValue("name").ToString();
            }
            catch (Exception e)
            {
                logger.LogTrace(e, $"Could not extract name from {asset.Path}");
            }

            return string.Empty;
        }
    }
}
