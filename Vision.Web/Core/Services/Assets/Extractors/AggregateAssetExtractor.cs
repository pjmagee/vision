using System.Collections.Generic;
using System.Linq;

namespace Vision.Web.Core
{
    public class AggregateAssetExtractor : IAssetExtractor
    {
        private readonly IEnumerable<IAssetExtractor> extractionServices;        

        public AggregateAssetExtractor(NpmAssetExtractor npmExtractionService, NuGetAssetExtractor nuGetPackageExtractionService, DockerAssetExtractor dockerAssetExtractor)
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

        public string ExtractPublishName(Asset asset)
        {
            return extractionServices
                .Where(service => service.Supports(asset.Kind))
                .Select(x => x.ExtractPublishName(asset)).FirstOrDefault();
        }

        public bool Supports(DependencyKind kind)
        {
            // we don't really care, this is the top level aggregate extractor
            return extractionServices.Any(es => es.Supports(kind));
        }
    }
}
