using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vision.Core
{
    public class PyPiVersionProvider : IDependencyVersionProvider
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger<PyPiVersionProvider> logger;

        public PyPiVersionProvider(ILogger<PyPiVersionProvider> logger)
        {
            this.logger = logger;
        }

        public bool Supports(EcosystemKind kind) => kind == EcosystemKind.PyPi;

        public async Task<DependencyVersion> GetLatestMetaDataAsync(RegistryDto registry, Dependency dependency)
        {
            // https://pypi.org/pypi
            return await Task.FromResult(new DependencyVersion());
        }
    }
}
