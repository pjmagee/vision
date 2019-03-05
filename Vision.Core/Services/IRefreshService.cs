using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public interface IRefreshService
    {
        Task NextRefreshTaskAsync();
        Task RefreshAllAsync();
        Task RefreshVersionControlByIdAsync(Guid sourceId);
        Task RefreshRepositoryByIdAsync(Guid repositoryId);
        Task RefreshAssetByIdAsync(Guid assetId);
        Task RefreshAssetDependenciesAsync(Guid assetId);
        Task RefreshAssetFrameworksAsync(Guid assetId);
    }

    public class RefreshService : IRefreshService
    {
        private readonly VisionDbContext context;

        private readonly IVcsChecker vcsChecker;
        private readonly IAssetExtractor assetExtractor;
        private readonly IVersionChecker versionChecker;
        private readonly ILogger<RefreshService> logger;

        public RefreshService(VisionDbContext context, IVcsChecker vcsChecker, IAssetExtractor assetExtractor, IVersionChecker versionChecker, ILogger<RefreshService> logger)
        {
            this.vcsChecker = vcsChecker;
            this.assetExtractor = assetExtractor;
            this.versionChecker = versionChecker;
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

        public async Task NextRefreshTaskAsync()
        {
            SystemTask task = await context.Tasks.OrderByDescending(t => t.Created).FirstOrDefaultAsync(t => t.Status == Shared.TaskStatus.Pending);

            if (task == null)
            {
                logger.LogInformation($"No tasks to process...");
                return;
            }

            try
            {
                task.Status = Shared.TaskStatus.Processing;
                context.Tasks.Update(task);

                await context.SaveChangesAsync();

                logger.LogInformation($"Refresh task {task.Scope.ToString()}:{task.Id} processing...");

                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    switch (task.Scope)
                    {
                        case TaskScope.Asset:
                            await RefreshAssetByIdAsync(task.TargetId);
                            break;
                        case TaskScope.Dependency:
                            await RefreshDependencyByIdAsync(task.TargetId);
                            break;
                        case TaskScope.Repository:
                            await RefreshRepositoryByIdAsync(task.TargetId);
                            break;
                        case TaskScope.VersionControl:
                            await RefreshVersionControlByIdAsync(task.TargetId);
                            break;
                    }

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error performing refresh task for {task.Scope.ToString()}:{task.TargetId}");
            }
            finally
            {
                task.Completed = DateTime.Now;
                task.Status = Shared.TaskStatus.Done;

                context.Tasks.Update(task);
                await context.SaveChangesAsync();
            }
        }

        public async Task RefreshRepositoryByIdAsync(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            logger.LogInformation($"Refreshing repository: {repository.Url}");
            await context.Entry(repository).Collection(r => r.Assets).LoadAsync();
            context.Assets.RemoveRange(repository.Assets);
            await context.SaveChangesAsync();

            IEnumerable<Asset> items = await vcsChecker.GetAssetsAsync(repository);

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

            IEnumerable<Repository> items = await vcsChecker.GetRepositoriesAsync(versionControl);

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
            DependencyKind kind = AppHelper.GetDependencyKind(asset);
            string version = extracted.Version;

            Framework framework = await context.Frameworks.FirstOrDefaultAsync(d => d.Version == version) ?? new Framework { Id = Guid.NewGuid(), Version = version };

            if (context.Entry(framework).State == EntityState.Detached)
            {
                context.Frameworks.Add(framework);
            }

            context.AssetFrameworks.Add(new AssetFramework { Id = Guid.NewGuid(), Asset = asset, AssetId = asset.Id, Framework = framework, FrameworkId = framework.Id });
            await context.SaveChangesAsync();
        }

        private async Task AssignAssetDependencies(Extract extracted, Asset asset)
        {
            DependencyKind kind = AppHelper.GetDependencyKind(asset);
            string version = extracted.Version;
            string name = extracted.Name;            

            Dependency dependency = await context.Dependencies.FirstOrDefaultAsync(d => d.Name == name && d.Kind == kind) ?? new Dependency { Id = Guid.NewGuid(), Kind = kind, Name = name };

            /* FIND AND SAVE DEPENDENCY IF NEEDED */
            if (context.Entry(dependency).State == EntityState.Detached)
            {
                context.Dependencies.Add(dependency);                
                await context.SaveChangesAsync();
            }

            /* FIND AND SAVE LATEST IF NEEDED */
            DependencyVersion latest = await versionChecker.GetLatestVersionAsync(dependency);

            // Save Latest version if not found
            if (await context.DependencyVersions.AllAsync(dv => dv.Version != latest.Version))
            {
                context.DependencyVersions.Add(latest);
            }

            /* FIND AND SAVE CURRENT VERSION IF NEEDED */
            DependencyVersion dependencyVersion = dependency.Versions.FirstOrDefault(v => v.Version == version) ?? new DependencyVersion { Id = Guid.NewGuid(), Dependency = dependency, Version = version  };
                        
            if (context.Entry(dependencyVersion).State == EntityState.Detached)
            {
                context.DependencyVersions.Add(dependencyVersion);
            }

            context.AssetDependencies.Add(new AssetDependency { Id = Guid.NewGuid(), Asset = asset, DependencyVersion = dependencyVersion });
        }        

        public async Task RefreshDependencyByIdAsync(Guid dependencyId)
        {
            Dependency dependency = await context.Dependencies.FindAsync(dependencyId);
            DependencyVersion latestVersion = await versionChecker.GetLatestVersionAsync(dependency);
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
