using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public class PyPiVersionProvider : IDependencyVersionProvider
    {
        private readonly ILogger<PyPiVersionProvider> logger;

        public PyPiVersionProvider(ILogger<PyPiVersionProvider> logger)
        {
            this.logger = logger;
        }

        public bool Supports(DependencyKind kind) => kind == DependencyKind.PyPi;

        public async Task<DependencyVersion> GetLatestMetaDataAsync(RegistryDto registry, Dependency dependency)
        {
            // https://pypi.org/pypi
            return await Task.FromResult(new DependencyVersion());
        }
    }
}
