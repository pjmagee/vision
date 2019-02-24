using System.Collections.Generic;
using System.Linq;

namespace Vision.Core
{
    public class AggregateExtractor : IDependencyExtractor
    {
        private readonly IEnumerable<IDependencyExtractor> extractionServices;        

        public AggregateExtractor(NodePackagesExtractor npmExtractionService, NuGetPackageExtractor nuGetPackageExtractionService)
        {
            extractionServices = new IDependencyExtractor[] 
            {
                npmExtractionService,
                nuGetPackageExtractionService
            };
        }

        public IEnumerable<Extract> ExtractDependencies(Asset asset) => extractionServices.SelectMany(service => service.ExtractDependencies(asset));

        public IEnumerable<Extract> ExtractFrameworks(Asset asset) => extractionServices.SelectMany(service => service.ExtractFrameworks(asset));
    }
}
