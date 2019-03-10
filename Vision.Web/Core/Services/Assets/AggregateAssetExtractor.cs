using System.Collections.Generic;
using System.Linq;

namespace Vision.Web.Core
{
    public class AggregateAssetExtractor : IAssetExtractor
    {
        private readonly IEnumerable<IAssetExtractor> extractionServices;        

        public AggregateAssetExtractor(
            NPMAssetExtractor npmExtractionService, 
            NuGetAssetExtractor nuGetPackageExtractionService, 
            DockerAssetExtractor dockerAssetExtractor)
        {
            extractionServices = new IAssetExtractor[] 
            {
                npmExtractionService,
                nuGetPackageExtractionService,
                dockerAssetExtractor
            };
        }

        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            return extractionServices
                .Where(service => service.Supports(asset.Kind))
                .SelectMany(service => service.ExtractDependencies(asset));
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset)
        {
            return extractionServices
                .Where(service => service.Supports(asset.Kind))
                .SelectMany(service => service.ExtractFrameworks(asset));
        }

        public bool Supports(DependencyKind kind)
        {
            return extractionServices.Any(es => es.Supports(kind));
        }
    }
}
