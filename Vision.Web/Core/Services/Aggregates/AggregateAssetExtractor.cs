using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vision.Web.Core
{
    public class AggregateAssetExtractor : IAggregateAssetExtractor
    {
        private readonly IEnumerable<IAssetExtractor> extractors;
        private readonly ILogger<AggregateAssetExtractor> logger;

        public AggregateAssetExtractor(IEnumerable<IAssetExtractor> extractors, ILogger<AggregateAssetExtractor> logger)
        {
            this.extractors = extractors;
            this.logger = logger;
        }

        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            using (var scope = logger.BeginScope($"{nameof(ExtractDependencies)}::[{asset.Path}]"))
            {
                foreach(var extractor in extractors.Where(extractor => extractor.Supports(asset.Kind)))
                {
                    try
                    {
                        var extracts = extractor.ExtractDependencies(asset);

                        logger.LogInformation($"{nameof(ExtractDependencies)}::['{asset.Path}']::[{extractor.GetType().Name}]::FOUND::[{extracts.Count()}]");

                        if (extracts.Any())
                        {                            
                            return extracts;
                        }                                                 
                    }
                    catch(Exception e)
                    {
                        logger.LogError(e, $"{nameof(ExtractDependencies)}::['{asset.Path}']::[{extractor.GetType().Name}]::ERROR");
                    }
                }
            }

            logger.LogInformation($"{nameof(ExtractDependencies)}::['{asset.Path}']::NONE");

            return Enumerable.Empty<Extract>();
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset)
        {
            using (var scope = logger.BeginScope($"{nameof(ExtractDependencies)}::[{asset.Path}]"))
            {
                foreach (var extractor in extractors.Where(extractor => extractor.Supports(asset.Kind)))
                {
                    try
                    {
                        var extracts = extractor.ExtractFrameworks(asset);

                        logger.LogInformation($"{nameof(ExtractFrameworks)}::['{asset.Path}']::[{extractor.GetType().Name}]::FOUND::[{extracts.Count()}]");

                        if (extracts.Any())
                        {
                            return extracts;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"{nameof(ExtractFrameworks)}::['{asset.Path}']::[{extractor.GetType().Name}]::ERROR");
                    }
                }
            }

            logger.LogInformation($"{nameof(ExtractFrameworks)}::['{asset.Path}']::NONE");

            return Enumerable.Empty<Extract>();
        }

        public string ExtractPublishName(Asset asset)
        {
            using (var scope = logger.BeginScope($"{nameof(ExtractDependencies)}::[{asset.Path}]"))
            {
                foreach (var extractor in extractors.Where(extractor => extractor.Supports(asset.Kind)))
                {
                    try
                    {
                        var name = extractor.ExtractPublishName(asset);

                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            logger.LogInformation($"{nameof(ExtractPublishName)}::['{asset.Path}']::[{extractor.GetType().Name}]::FOUND::[{name}]");
                            return name;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"{nameof(ExtractPublishName)}::['{asset.Path}']::[{extractor.GetType().Name}]::ERROR");
                    }
                }
            }

            logger.LogInformation($"{nameof(ExtractPublishName)}::['{asset.Path}']::NONE");

            return string.Empty;
        }
    }
}
