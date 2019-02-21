using System.Collections.Generic;
using System.Linq;

namespace Vision.Core
{
    public interface IExtractionService
    {
        IEnumerable<Extract> ExtractDependencies(Asset asset);
        IEnumerable<Extract> ExtractFrameworks(Asset asset);
    }

    public class AggregateExtractionService : IExtractionService
    {
        private readonly IEnumerable<IExtractionService> extractionServices;        

        public AggregateExtractionService(NpmExtractionService npmExtractionService, NuGetPackageExtractionService nuGetPackageExtractionService)
        {
            extractionServices = new IExtractionService[] { npmExtractionService, nuGetPackageExtractionService };
        }

        public IEnumerable<Extract> ExtractDependencies(Asset asset) => extractionServices.SelectMany(service => service.ExtractDependencies(asset));

        public IEnumerable<Extract> ExtractFrameworks(Asset asset) => extractionServices.SelectMany(service => service.ExtractFrameworks(asset));
    }
}
