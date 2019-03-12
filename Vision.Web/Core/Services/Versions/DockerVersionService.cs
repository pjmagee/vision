namespace Vision.Web.Core
{
    using Docker.Registry.DotNet;
    using Docker.Registry.DotNet.Authentication;
    using Docker.Registry.DotNet.Models;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    public class DockerVersionService : VersionService
    {
        public DockerVersionService(VisionDbContext context, IDataProtectionProvider provider, ILogger<VersionService> logger) : base(context, provider, logger)
        {

        }

        public override bool Supports(DependencyKind kind) => kind == DependencyKind.Docker;

        protected override async Task<DependencyVersion> GetLatestVersionAsync(Registry registry, Dependency dependency)
        {
            // RegistryClientConfiguration clientConfig = new RegistryClientConfiguration(registry.Endpoint);
            //if (!string.IsNullOrWhiteSpace(registry.Username) && !string.IsNullOrWhiteSpace(registry.Password))
            //{
            //    using (IRegistryClient client = clientConfig.CreateClient(new PasswordOAuthAuthenticationProvider(registry.Username, registry.Password)))
            //    {
            //        ListImageTagsResponse imageTags = await client.Tags.ListImageTagsAsync(dependency.Name, new ListImageTagsParameters());
            //        return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = imageTags.Name };
            //    }
            //}
            //else
            //{
            //    using (IRegistryClient client = clientConfig.CreateClient())
            //    {
            //        ListImageTagsResponse imageTags = await client.Tags.ListImageTagsAsync(dependency.Name, new ListImageTagsParameters());
            //        return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = imageTags.Name };
            //    }
            //}

            RegistryClientConfiguration clientConfig = new RegistryClientConfiguration(new Uri(registry.Endpoint).GetComponents(UriComponents.HostAndPort, UriFormat.SafeUnescaped));

            using (IRegistryClient client = clientConfig.CreateClient())
            {
                ListImageTagsResponse imageTags = await client.Tags.ListImageTagsAsync(dependency.Name, new ListImageTagsParameters());
                return new DependencyVersion { Dependency = dependency, Version = imageTags.Tags[imageTags.Tags.Length - 1] };
            }
        }
    }
}
