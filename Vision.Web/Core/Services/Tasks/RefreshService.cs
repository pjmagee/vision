using Microsoft.EntityFrameworkCore;
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
            foreach (Vcs source in context.VcsSources)
            {
                await RefreshVersionControlByIdAsync(source.Id);
            }
        }

        public async Task RefreshRepositoryByIdAsync(Guid repositoryId)
        {
            VcsRepository repository = await context.VcsRepositories.FindAsync(repositoryId);
            Vcs versionControl = await context.VcsSources.FindAsync(repository.VcsId);

            logger.LogInformation($"Refreshing repository: {repository.Url}");
            await context.Entry(repository).Collection(r => r.Assets).LoadAsync();
            context.Assets.RemoveRange(repository.Assets);
            await context.SaveChangesAsync();

            var versionControlDto = new VersionControlDto { ApiKey = versionControl.ApiKey, Endpoint = versionControl.Endpoint, VcsId = versionControl.Id, Kind = versionControl.Kind, IsEnabled = versionControl.IsEnabled };
            var repositoryDto = new RepositoryDto { VcsId = repository.VcsId, Url = repository.Url, WebUrl = repository.WebUrl };

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
            context.AssetEcoSystems.RemoveRange(asset.AssetEcosystem);
            await context.SaveChangesAsync();

            IEnumerable<Extract> extracts = assetExtractor.ExtractRuntimes(asset);

            foreach (Extract extract in extracts)
            {
                await AssignAssetToRuntime(extract, asset);
            }
        }

        public async Task RefreshVersionControlByIdAsync(Guid VcsId)
        {
            Vcs versionControl = await context.VcsSources.FindAsync(VcsId);

            await context.Entry(versionControl).Collection(vcs => vcs.Repositories).LoadAsync();
            context.VcsRepositories.RemoveRange(versionControl.Repositories.Where(r => r.IsIgnored == false));
            await context.SaveChangesAsync();

            var versionControlDto = new VersionControlDto { ApiKey = versionControl.ApiKey, Endpoint = versionControl.Endpoint, Kind = versionControl.Kind, VcsId = versionControl.Id };
            encryptionService.Decrypt(versionControlDto);

            IEnumerable<VcsRepository> items = await this.versionControlProvider.GetRepositoriesAsync(versionControlDto);

            foreach (VcsRepository repository in items)
            {
                context.VcsRepositories.Add(repository);
                await context.SaveChangesAsync();
            }

            foreach (var repository in context.VcsRepositories.Where(r => r.VcsId == VcsId))
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
            EcosystemVersion runtime = await GetOrCreateRuntime(extracted);

            context.AssetEcoSystems.Add(new AssetEcosystem { Asset = asset, AssetId = asset.Id, EcosystemVersion = runtime, EcosystemVersionId = runtime.Id });

            await context.SaveChangesAsync();
        }

        private async Task<EcosystemVersion> GetOrCreateRuntime(Extract extract)
        {
            Ecosystem runtime = await context.Ecosystems.FirstAsync(r => r.Name == extract.EcosystemIdentifier);
            EcosystemVersion version = runtime.EcosystemVersions.FirstOrDefault(d => d.Version == extract.EcosystemVersion) ?? new EcosystemVersion { Id = Guid.NewGuid(), Ecosystem = runtime, Version = extract.EcosystemVersion };

            if (context.Entry(version).State == EntityState.Detached)
            {
                context.EcosystemVersions.Add(version);
                await context.SaveChangesAsync();
            }

            return version;
        }

        private async Task AssignAssetDependencies(Extract extract, Asset asset)
        {
            string version = extract.EcosystemVersion;
            string name = extract.EcosystemIdentifier;

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

        private async Task<Dependency> GetOrCreateDependency(EcosystemKind kind, string name)
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
