using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public interface ISystemRefreshService
    {
        Task RefreshAllAsync();
        Task RefreshSourceAsync(Guid sourceId);
        Task RefreshRepositoryAsync(Guid repositoryId);
        Task RefreshAssetAsync(Guid assetId);
        Task RefreshAssetDependenciesAsync(Guid assetId);
        Task RefreshAssetFrameworksAsync(Guid assetId);
    }

    public class SystemRefreshService : ISystemRefreshService
    {
        private readonly VisionDbContext context;

        private readonly IGitService gitService;
        private readonly IExtractionService extractionService;
        private readonly IVersionService versionService;

        public SystemRefreshService(VisionDbContext context, IGitService gitService, IExtractionService extractionService, IVersionService versionService)
        {
            this.gitService = gitService;
            this.extractionService = extractionService;
            this.versionService = versionService;
            this.context = context;
        }

        public async Task RefreshAllAsync()
        {
            using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
            {
                foreach (GitSource source in context.GitSources)
                {
                    await RefreshSourceAsync(source.Id);
                }

                transaction.Commit();
            }
        }

        public async Task RefreshRepositoryAsync(Guid repositoryId)
        {
            GitRepository repository = await context.GitRepositories.FindAsync(repositoryId);
            context.Assets.RemoveRange(repository.Assets);
            await context.SaveChangesAsync();

            IEnumerable<Asset> items = await gitService.GetAssetsAsync(repository);

            foreach(Asset asset in items)
            {                
                context.Assets.Add(asset);
                await context.SaveChangesAsync();
                await RefreshAssetAsync(asset.Id);
            }
        }

        public async Task RefreshAssetAsync(Guid assetId)
        {           
            await RefreshAssetDependenciesAsync(assetId);
            await RefreshAssetFrameworksAsync(assetId);
            await context.SaveChangesAsync();
        }

        public async Task RefreshAssetDependenciesAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);
            context.AssetDependencies.RemoveRange(asset.Dependencies);
            await context.SaveChangesAsync();

            IEnumerable<Extract> items = extractionService.ExtractDependencies(asset);

            foreach(Extract extract in items)
            {
                await AssignAssetDependencies(extract, asset);
            }
        }

        public async Task RefreshAssetFrameworksAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);
            context.AssetFrameworks.RemoveRange(asset.Frameworks);
            await context.SaveChangesAsync();
            
            IEnumerable<Extract> items = extractionService.ExtractFrameworks(asset);

            foreach(Extract extract in items)
            {
                await AssignAssetFrameworksAsync(extract, asset);
            }
        }

        public async Task RefreshSourceAsync(Guid sourceId)
        {
            GitSource source = await context.GitSources.FindAsync(sourceId);
            context.GitRepositories.RemoveRange(source.GitRepositories);
            await context.SaveChangesAsync();

            IEnumerable<GitRepository> items = await gitService.GetRepositoriesAsync(source);

            foreach(GitRepository repository in items)
            {
                context.GitRepositories.Add(repository);
                await context.SaveChangesAsync();
                await RefreshRepositoryAsync(repository.Id);
            }
        }

        private async Task AssignAssetFrameworksAsync(Extract extracted, Asset asset)
        {
            DependencyKind kind = AssetHelper.GetDependencyKind(asset);
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
            DependencyKind kind = AssetHelper.GetDependencyKind(asset);
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
            DependencyVersion latest = await versionService.GetLatestVersion(dependency);

            // Save Latest version if not found
            if (await context.DependencyVersions.AnyAsync(v => v.Version == latest.Version))
            {
                context.DependencyVersions.Add(latest);
            }

            /* FIND AND SAVE CURRENT VERSION IF NEEDED */
            DependencyVersion dependencyVersion = dependency.Versions.FirstOrDefault(v => v.Version == version) ?? new DependencyVersion { Id = Guid.NewGuid(), Dependency = dependency, Version = version, IsVulnerable = false, VulnerabilityUrl = null };
                        
            if (context.Entry(dependencyVersion).State == EntityState.Detached)
            {
                context.DependencyVersions.Add(dependencyVersion);
            }

            context.AssetDependencies.Add(new AssetDependency { Id = Guid.NewGuid(), Asset = asset, DependencyVersion = dependencyVersion });
        }
    }   
}
