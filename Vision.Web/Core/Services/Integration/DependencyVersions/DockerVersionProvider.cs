namespace Vision.Web.Core
{
    using Docker.Registry.DotNet;
    using Docker.Registry.DotNet.Authentication;
    using Docker.Registry.DotNet.Models;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class DockerVersionProvider : IDependencyVersionProvider
    {
        private readonly ILogger<DockerVersionProvider> logger;

        public DockerVersionProvider(ILogger<DockerVersionProvider> logger)
        {
            this.logger = logger;
        }

        public bool Supports(EcosystemKind kind) => kind == EcosystemKind.Docker;

        public async Task<DependencyVersion> GetLatestMetaDataAsync(RegistryDto registry, Dependency dependency)
        {
            RegistryClientConfiguration clientConfig = new RegistryClientConfiguration(new Uri(registry.Endpoint).GetComponents(UriComponents.HostAndPort, UriFormat.SafeUnescaped));

            DependencyVersion version;

            if (!string.IsNullOrWhiteSpace(registry.Username) && !string.IsNullOrWhiteSpace(registry.Password))
            {
                using (IRegistryClient client = clientConfig.CreateClient(new PasswordOAuthAuthenticationProvider(registry.Username, registry.Password)))
                {
                    ListImageTagsResponse imageTags = await client.Tags.ListImageTagsAsync(dependency.Name, new ListImageTagsParameters());
                    version = new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = imageTags.Tags.Last(), IsLatest = true };
                }
            }
            else
            {
                using (IRegistryClient client = clientConfig.CreateClient())
                {
                    ListImageTagsResponse imageTags = await client.Tags.ListImageTagsAsync(dependency.Name, new ListImageTagsParameters());
                    version = new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = imageTags.Tags.Last(), IsLatest = true };
                }
            }

            return version;
        }
    }
}
