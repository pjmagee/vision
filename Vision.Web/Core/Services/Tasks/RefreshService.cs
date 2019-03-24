namespace Vision.Web.Core
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
            using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
            {
                foreach (VersionControl source in context.VersionControls)
                {
                    await RefreshVersionControlByIdAsync(source.Id);
                }

                transaction.Commit();
            }
        }        

        public async Task RefreshRepositoryByIdAsync(Guid repositoryId)
        {
            using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
            {
                Repository repository = await context.Repositories.FindAsync(repositoryId);
                VersionControl versionControl = await context.VersionControls.FindAsync(repository.VersionControlId);

                logger.LogInformation($"Refreshing repository: {repository.Url}");
                await context.Entry(repository).Collection(r => r.Assets).LoadAsync();
                context.Assets.RemoveRange(repository.Assets);
                await context.SaveChangesAsync();

                var versionControlDto = new VersionControlDto { ApiKey = versionControl.ApiKey, Endpoint = versionControl.Endpoint, VersionControlId = versionControl.Id, Kind = versionControl.Kind, IsEnabled = versionControl.IsEnabled };
                var repositoryDto = new RepositoryDto { VersionControlId = repository.VersionControlId, Url = repository.Url, WebUrl = repository.WebUrl };

                encryptionService.Decrypt(versionControlDto);
                IEnumerable<Asset> items = await versionControlProvider.GetAssetsAsync(versionControlDto, repositoryDto);

                foreach (Asset asset in items)
                {
                    context.Assets.Add(asset);
                    await context.SaveChangesAsync();
                    await RefreshAssetByIdAsync(asset.Id);
                }

                transaction.Commit();
            }                           
        }

        public async Task RefreshAssetByIdAsync(Guid assetId)
        {
            using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
            {
                await RefreshAssetDependenciesAsync(assetId);
                await RefreshAssetFrameworksAsync(assetId);
                await context.SaveChangesAsync();

                transaction.Commit();
            }   
        }

        public async Task RefreshAssetDependenciesAsync(Guid assetId)
        {
            using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
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

                transaction.Commit();
            }
        }

        public async Task RefreshAssetFrameworksAsync(Guid assetId)
        {
            using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
            {
                Asset asset = await context.Assets.FindAsync(assetId);

                logger.LogInformation($"Refreshing asset frameworks for: {asset.Path}");

                await context.Entry(asset).Collection(a => a.Frameworks).LoadAsync();
                context.AssetFrameworks.RemoveRange(asset.Frameworks);
                await context.SaveChangesAsync();

                IEnumerable<Extract> extracts = assetExtractor.ExtractFrameworks(asset);

                foreach (var extract in extracts)
                {
                    await AssignAssetFrameworksAsync(extract, asset);
                }

                transaction.Commit();
            }           
        }

        public async Task RefreshVersionControlByIdAsync(Guid versionControlId)
        {
            using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
            {
                VersionControl versionControl = await context.VersionControls.FindAsync(versionControlId);

                await context.Entry(versionControl).Collection(vcs => vcs.Repositories).LoadAsync();
                context.Repositories.RemoveRange(versionControl.Repositories.Where(r => r.IsIgnored == false));
                await context.SaveChangesAsync();

                var versionControlDto = new VersionControlDto { ApiKey = versionControl.ApiKey, Endpoint = versionControl.Endpoint, Kind = versionControl.Kind, VersionControlId = versionControl.Id };
                encryptionService.Decrypt(versionControlDto);

                IEnumerable<Repository> items = await this.versionControlProvider.GetRepositoriesAsync(versionControlDto);

                foreach (Repository repository in items)
                {
                    context.Repositories.Add(repository);
                    await context.SaveChangesAsync();
                }

                foreach (var repository in context.Repositories.Where(r => r.VersionControlId == versionControlId))
                {
                    await RefreshRepositoryByIdAsync(repository.Id);
                }

                transaction.Commit();
            }            
        }

        public async Task RefreshDependencyByIdAsync(Guid dependencyId)
        {
            using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
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

                // TODO: Remove versions not used by any assets..

                transaction.Commit();
            }
        }

        private async Task AssignAssetFrameworksAsync(Extract extracted, Asset asset)
        {            
            Framework framework = await GetOrCreateFramework(extracted);

            if (context.Entry(framework).State == EntityState.Detached)
            {
                context.Frameworks.Add(framework);
            }

            context.AssetFrameworks.Add(new AssetFramework { Asset = asset, AssetId = asset.Id, Framework = framework, FrameworkId = framework.Id });
            await context.SaveChangesAsync();
        }

        private async Task<Framework> GetOrCreateFramework(Extract extract)
        {
            return await context.Frameworks.FirstOrDefaultAsync(d => d.Name == extract.Name && d.Version == extract.Version) ?? new Framework { Name = extract.Name, Version = extract.Version };
        }

        private async Task AssignAssetDependencies(Extract extract, Asset asset)
        {
            string version = extract.Version;
            string name = extract.Name;

            Dependency dependency = await GetOrCreateDependency(asset.Kind, name);

            /* FIND AND SAVE DEPENDENCY IF NEEDED */
            if (context.Entry(dependency).State == EntityState.Detached)
            {
                context.Dependencies.Add(dependency);
                await context.SaveChangesAsync();
            }

            DependencyVersion latest = await versionProvider.GetLatestMetaDataAsync(dependency);

            if (await context.DependencyVersions.AllAsync(dv => dv.Version != latest.Version))
            {
                context.DependencyVersions.Add(latest);
                await context.SaveChangesAsync();
            }

            DependencyVersion dependencyVersion = await GetOrCreateDependencyVersion(dependency, version);

            if (context.Entry(dependencyVersion).State == EntityState.Detached)
            {
                context.DependencyVersions.Add(dependencyVersion);
            }

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
            return await context.Dependencies.
                FirstOrDefaultAsync(d => d.Name == name && d.Kind == kind) ?? 
                new Dependency { Name = name, Kind = kind, Updated = DateTime.Now };
        }

        private async Task<DependencyVersion> GetOrCreateDependencyVersion(Dependency dependency, string version)
        {
            DependencyVersion dependencyVersion = await context.DependencyVersions.FirstOrDefaultAsync(v => v.DependencyId == dependency.Id && v.Version == version);
            return (dependencyVersion ?? new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = version });
        }

        
    }   
}
