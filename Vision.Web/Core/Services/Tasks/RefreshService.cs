using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public class RefreshService : IRefreshService
    {
        private readonly VisionDbContext context;

        private readonly IAggregateVersionControlProvider versionControlProvider;
        private readonly IAggregateDependencyVersionProvider versionProvider;
        private readonly IAggregateAssetExtractor assetExtractor;
        private readonly IEncryptionService encryptionService;
        private readonly ILogger<RefreshService> logger;

        public RefreshService(
            VisionDbContext context,
            IEncryptionService encryptionService,
            IAggregateVersionControlProvider versionControlProvider,
            IAggregateAssetExtractor assetExtractor,
            IAggregateDependencyVersionProvider versionProvider,
            ILogger<RefreshService> logger)
        {
            this.versionControlProvider = versionControlProvider;
            this.assetExtractor = assetExtractor;
            this.versionProvider = versionProvider;
            this.encryptionService = encryptionService;
            this.logger = logger;
            this.context = context;
        }

        public async Task RefreshAllAsync()
        {
            foreach (Vcs source in context.VersionControls)
            {
                await RefreshVersionControlByIdAsync(source.Id);
            }
        }

        public async Task RefreshRepositoryByIdAsync(Guid repositoryId)
        {
            VcsRepository repository = await context.Repositories.FindAsync(repositoryId);
            Vcs versionControl = await context.VersionControls.FindAsync(repository.VcsId);

            logger.LogInformation($"Refreshing repository: {repository.Url}");
            await context.Entry(repository).Collection(r => r.Assets).LoadAsync();
            context.Assets.RemoveRange(repository.Assets);
            await context.SaveChangesAsync();

            var versionControlDto = new VersionControlDto { ApiKey = versionControl.ApiKey, Endpoint = versionControl.Endpoint, VersionControlId = versionControl.Id, Kind = versionControl.Kind, IsEnabled = versionControl.IsEnabled };
            var repositoryDto = new RepositoryDto { VersionControlId = repository.VcsId, Url = repository.Url, WebUrl = repository.WebUrl };

            encryptionService.Decrypt(versionControlDto);
            IEnumerable<Asset> items = await versionControlProvider.GetAssetsAsync(versionControlDto, repositoryDto);

            foreach (Asset asset in items)
            {
                context.Assets.Add(asset);
                await context.SaveChangesAsync();
                await RefreshAssetByIdAsync(asset.Id);
            }
        }

        public async Task RefreshAssetByIdAsync(Guid assetId)
        {
            await RefreshAssetDependenciesAsync(assetId);
            await RefreshAssetFrameworksAsync(assetId);
            await context.SaveChangesAsync();
        }

        public async Task RefreshAssetDependenciesAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);

            logger.LogInformation($"Refreshing asset dependencies for: {asset.Path}");
            await context.Entry(asset).Collection(a => a.Dependencies).LoadAsync();
            context.AssetDependencies.RemoveRange(asset.Dependencies);
            await context.SaveChangesAsync();

            IEnumerable<Extract> items = assetExtractor.ExtractDependencies(asset);

            foreach (Extract extract in items)
            {
                await AssignAssetDependencies(extract, asset);
            }
        }

        public async Task RefreshAssetFrameworksAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);

            logger.LogInformation($"Refreshing asset frameworks for: {asset.Path}");

            //await context.Entry(asset).Collection(a => a.).LoadAsync();
            context.AssetRuntimes.RemoveRange(asset.AssetRuntime);
            await context.SaveChangesAsync();

            IEnumerable<Extract> extracts = assetExtractor.ExtractRuntimes(asset);

            foreach (Extract extract in extracts)
            {
                await AssignAssetToRuntime(extract, asset);
            }
        }

        public async Task RefreshVersionControlByIdAsync(Guid versionControlId)
        {
            Vcs versionControl = await context.VersionControls.FindAsync(versionControlId);

            await context.Entry(versionControl).Collection(vcs => vcs.Repositories).LoadAsync();
            context.Repositories.RemoveRange(versionControl.Repositories.Where(r => r.IsIgnored == false));
            await context.SaveChangesAsync();

            var versionControlDto = new VersionControlDto { ApiKey = versionControl.ApiKey, Endpoint = versionControl.Endpoint, Kind = versionControl.Kind, VersionControlId = versionControl.Id };
            encryptionService.Decrypt(versionControlDto);

            IEnumerable<VcsRepository> items = await this.versionControlProvider.GetRepositoriesAsync(versionControlDto);

            foreach (VcsRepository repository in items)
            {
                context.Repositories.Add(repository);
                await context.SaveChangesAsync();
            }

            foreach (var repository in context.Repositories.Where(r => r.VcsId == versionControlId))
            {
                await RefreshRepositoryByIdAsync(repository.Id);
            }
        }

        public async Task RefreshDependencyByIdAsync(Guid dependencyId)
        {
            Dependency dependency = await context.Dependencies.FindAsync(dependencyId);
            DependencyVersion latestVersion = await versionProvider.GetLatestMetaDataAsync(dependency);
            List<DependencyVersion> currentVersions = await context.DependencyVersions.Where(x => x.DependencyId == dependencyId).ToListAsync();

            if (currentVersions.All(current => current.Version != latestVersion.Version))
            {
                currentVersions.ForEach(v => v.IsLatest = false);
                context.DependencyVersions.UpdateRange(currentVersions);
                context.DependencyVersions.Add(latestVersion);
            }

            await context.SaveChangesAsync();
        }

        private async Task AssignAssetToRuntime(Extract extracted, Asset asset)
        {
            RuntimeVersion runtime = await GetOrCreateRuntime(extracted);

            context.AssetRuntimes.Add(new AssetRuntime { Asset = asset, AssetId = asset.Id, RuntimeVersion = runtime, RuntimeVersionId = runtime.Id });

            await context.SaveChangesAsync();
        }

        private async Task<RuntimeVersion> GetOrCreateRuntime(Extract extract)
        {
            Runtime runtime = await context.Runtimes.FirstAsync(r => r.Name == extract.RuntimeIdentifier);
            RuntimeVersion version = runtime.Versions.FirstOrDefault(d => d.Version == extract.RuntimeVersion) ?? new RuntimeVersion { Id = Guid.NewGuid(), Runtime = runtime, Version = extract.RuntimeVersion };

            if (context.Entry(version).State == EntityState.Detached)
            {
                context.RuntimeVersions.Add(version);
                await context.SaveChangesAsync();
            }

            return version;
        }

        private async Task AssignAssetDependencies(Extract extract, Asset asset)
        {
            string version = extract.RuntimeVersion;
            string name = extract.RuntimeIdentifier;

            Dependency dependency = await GetOrCreateDependency(asset.Kind, name);
            DependencyVersion latest = await versionProvider.GetLatestMetaDataAsync(dependency);

            if (await context.DependencyVersions.AllAsync(dv => dv.Version != latest.Version))
            {
                context.DependencyVersions.Add(latest);
                await context.SaveChangesAsync();
            }

            DependencyVersion dependencyVersion = await GetOrCreateDependencyVersion(dependency, version);

            context.AssetDependencies.Add(new AssetDependency
            {
                AssetId = asset.Id,
                DependencyId = dependency.Id,
                DependencyVersionId = dependencyVersion.Id,
                Asset = asset,
                DependencyVersion = dependencyVersion,
                Dependency = dependency
            });
        }

        private async Task<Dependency> GetOrCreateDependency(DependencyKind kind, string name)
        {
            Dependency dependency = await context.Dependencies.FirstOrDefaultAsync(d => d.Name == name && d.Kind == kind) ?? new Dependency { Name = name, Kind = kind, Updated = DateTime.Now };

            if (context.Entry(dependency).State == EntityState.Detached)
            {
                context.Dependencies.Add(dependency);
                await context.SaveChangesAsync();
            }

            return dependency;
        }

        private async Task<DependencyVersion> GetOrCreateDependencyVersion(Dependency dependency, string version)
        {
            DependencyVersion dependencyVersion = await context.DependencyVersions.FirstOrDefaultAsync(v => v.DependencyId == dependency.Id && v.Version == version) ?? new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = version };

            if (context.Entry(dependencyVersion).State == EntityState.Detached)
            {
                context.DependencyVersions.Add(dependencyVersion);
                await context.SaveChangesAsync();
            }

            return dependencyVersion;
        }
    }
}
