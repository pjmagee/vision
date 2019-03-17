﻿namespace Vision.Web.Core
{
    using Docker.Registry.DotNet;
    using Docker.Registry.DotNet.Authentication;
    using Docker.Registry.DotNet.Models;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class DockerVersionProvider : AbstractVersionProvider
    {
        public DockerVersionProvider(VisionDbContext context, IDataProtectionProvider provider, ILogger<DockerVersionProvider> logger) : base(context, provider, logger)
        {

        }

        public override bool Supports(DependencyKind kind) => kind == DependencyKind.Docker;

        protected override async Task<DependencyVersion> GetLatestMetaDataAsync(Registry registry, Dependency dependency)
        {
            RegistryClientConfiguration clientConfig = new RegistryClientConfiguration(new Uri(registry.Endpoint).GetComponents(UriComponents.HostAndPort, UriFormat.SafeUnescaped));

            if (!string.IsNullOrWhiteSpace(registry.Username) && !string.IsNullOrWhiteSpace(registry.Password))
            {
                using (IRegistryClient client = clientConfig.CreateClient(new PasswordOAuthAuthenticationProvider(registry.Username, registry.Password)))
                {
                    ListImageTagsResponse imageTags = await client.Tags.ListImageTagsAsync(dependency.Name, new ListImageTagsParameters());
                    return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = imageTags.Tags.Last() };
                }
            }
            else
            {
                using (IRegistryClient client = clientConfig.CreateClient())
                {
                    ListImageTagsResponse imageTags = await client.Tags.ListImageTagsAsync(dependency.Name, new ListImageTagsParameters());
                    return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = imageTags.Tags.Last() };
                }
            }
        }
    }
}