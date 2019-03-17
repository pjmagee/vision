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
        private readonly IVersionControlProvider versionControlService;
        private readonly IVersionProvider versionService;
        private readonly IAssetExtractor assetExtractor;        
        private readonly ILogger<RefreshService> logger;

        public RefreshService(
            VisionDbContext context,
            IVersionControlProvider versionControlService,
            IAssetExtractor assetExtractor,
            IVersionProvider versionService,
            ILogger<RefreshService> logger)
        {
            this.versionControlService = versionControlService;
            this.assetExtractor = assetExtractor;
            this.versionService = versionService;
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
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            logger.LogInformation($"Refreshing repository: {repository.Url}");
            await context.Entry(repository).Collection(r => r.Assets).LoadAsync();
            context.Assets.RemoveRange(repository.Assets);
            await context.SaveChangesAsync();

            IEnumerable<Asset> items = await versionControlService.GetAssetsAsync(repository);

            foreach(Asset asset in items)
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

            foreach(Extract extract in items)
            {
                await AssignAssetDependencies(extract, asset);
            }
        }

        public async Task RefreshAssetFrameworksAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);

            logger.LogInformation($"Refreshing asset frameworks for: {asset.Path}");

            await context.Entry(asset).Collection(a => a.Frameworks).LoadAsync();
            context.AssetFrameworks.RemoveRange(asset.Frameworks);
            await context.SaveChangesAsync();
            
            IEnumerable<Extract> items = assetExtractor.ExtractFrameworks(asset);

            foreach(Extract extract in items)
            {
                await AssignAssetFrameworksAsync(extract, asset);
            }
        }

        public async Task RefreshVersionControlByIdAsync(Guid versionControlId)
        {
            VersionControl versionControl = await context.VersionControls.FindAsync(versionControlId);

            await context.Entry(versionControl).Collection(vcs => vcs.Repositories).LoadAsync();            
            context.Repositories.RemoveRange(versionControl.Repositories);
            await context.SaveChangesAsync();

            IEnumerable<Repository> items = await versionControlService.GetRepositoriesAsync(versionControl);

            foreach(Repository repository in items)
            {
                context.Repositories.Add(repository);
                await context.SaveChangesAsync();                
            }

            foreach(var repository in context.Repositories.Where(r => r.VersionControlId == versionControlId))
            {
                await RefreshRepositoryByIdAsync(repository.Id);
            }
        }

        private async Task AssignAssetFrameworksAsync(Extract extracted, Asset asset)
        {            
            string version = extracted.Version;
            Framework framework = await GetOrCreateFramework(version);

            if (context.Entry(framework).State == EntityState.Detached)
            {
                context.Frameworks.Add(framework);
            }

            context.AssetFrameworks.Add(new AssetFramework { Asset = asset, AssetId = asset.Id, Framework = framework, FrameworkId = framework.Id });
            await context.SaveChangesAsync();
        }

        private async Task<Framework> GetOrCreateFramework(string version)
        {
            return await context.Frameworks.FirstOrDefaultAsync(d => d.Version == version) ?? new Framework { Version = version };
        }

        private async Task AssignAssetDependencies(Extract extracted, Asset asset)
        {
            string version = extracted.Version;
            string name = extracted.Name;

            Dependency dependency = await GetOrCreateDependency(asset.Kind, name);

            /* FIND AND SAVE DEPENDENCY IF NEEDED */
            if (context.Entry(dependency).State == EntityState.Detached)
            {
                context.Dependencies.Add(dependency);
                await context.SaveChangesAsync();
            }

            DependencyVersion latest = await versionService.GetLatestMetaDataAsync(dependency);

            if (await context.DependencyVersions.AllAsync(dv => dv.Version != latest.Version))
            {
                context.DependencyVersions.Add(latest);
                await context.SaveChangesAsync();
            }

            DependencyVersion dependencyVersion = await GetOrCreateDependencyVersion(version, dependency);

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

        private async Task<DependencyVersion> GetOrCreateDependencyVersion(string version, Dependency dependency)
        {

            /* FIND AND SAVE CURRENT VERSION IF NEEDED */
            return await context.DependencyVersions
                .FirstOrDefaultAsync(v => v.DependencyId == dependency.Id && v.Version == version) ?? 
                new DependencyVersion
                {
                    Dependency = dependency,
                    DependencyId = dependency.Id,
                    Version = version
                };
        }

        public async Task RefreshDependencyByIdAsync(Guid dependencyId)
        {
            Dependency dependency = await context.Dependencies.FindAsync(dependencyId);
            DependencyVersion latestVersion = await versionService.GetLatestMetaDataAsync(dependency);
            List<DependencyVersion> currentVersions = await context.DependencyVersions.Where(x => x.DependencyId == dependencyId).ToListAsync();

            if (currentVersions.All(current => current.Version != latestVersion.Version))
            {
                currentVersions.ForEach(v => v.IsLatest = false);
                context.DependencyVersions.UpdateRange(currentVersions);
                context.DependencyVersions.Add(latestVersion);
            }

            await context.SaveChangesAsync();

            // TODO: Remove versions not used by any assets..
        }
    }   
}
