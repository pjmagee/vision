using System.Collections.Generic;

namespace Vision.Core
{
    public interface IExtractionService
    {
        IEnumerable<Extract> ExtractDependencies(Asset asset);
        IEnumerable<Extract> ExtractFrameworks(Asset asset);
    }

    public class AggregateExtractionService : IExtractionService
    {
        private readonly NpmExtractionService npmExtractionService;
        private readonly NuGetPackageExtractionService nuGetPackageExtractionService;

        public AggregateExtractionService(NpmExtractionService npmExtractionService, NuGetPackageExtractionService nuGetPackageExtractionService)
        {
            this.npmExtractionService = npmExtractionService;
            this.nuGetPackageExtractionService = nuGetPackageExtractionService;
        }

        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset)
        {
            throw new System.NotImplementedException();
        }
    }
}
