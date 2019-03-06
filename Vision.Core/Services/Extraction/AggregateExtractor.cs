using System.Collections.Generic;
using System.Linq;

namespace Vision.Core
{
    public class AggregateExtractor : IAssetExtractor
    {
        private readonly IEnumerable<IAssetExtractor> extractionServices;        

        public AggregateExtractor(NodePackagesExtractor npmExtractionService, CSharpProjectExtractor nuGetPackageExtractionService)
        {
            extractionServices = new IAssetExtractor[] 
            {
                npmExtractionService,
                nuGetPackageExtractionService
            };
        }

        public IEnumerable<Extract> ExtractDependencies(Asset asset) => extractionServices.SelectMany(service => service.ExtractDependencies(asset));

        public IEnumerable<Extract> ExtractFrameworks(Asset asset) => extractionServices.SelectMany(service => service.ExtractFrameworks(asset));
    }
}
