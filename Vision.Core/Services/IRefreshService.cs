﻿using Microsoft.EntityFrameworkCore;
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
        Task RefreshGitSourceAsync(Guid sourceId);
        Task RefreshGitRepositoryId(Guid repositoryId);
        Task RefreshAssetAsync(Guid assetId);
        Task RefreshAssetDependenciesAsync(Guid assetId);
        Task RefreshAssetFrameworksAsync(Guid assetId);
    }

    public class RefreshService : IRefreshService
    {
        private readonly VisionDbContext context;

        private readonly IGitService gitService;
        private readonly IExtractionService extractionService;
        private readonly IVersionService versionService;
        private readonly ILogger<RefreshService> logger;

        public RefreshService(VisionDbContext context, IGitService gitService, IExtractionService extractionService, IVersionService versionService, ILogger<RefreshService> logger)
        {
            this.gitService = gitService;
            this.extractionService = extractionService;
            this.versionService = versionService;
            this.logger = logger;
            this.context = context;
        }

        public async Task RefreshAllAsync()
        {
            using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
            {
                foreach (GitSource source in context.GitSources)
                {
                    await RefreshGitSourceAsync(source.Id);
                }

                transaction.Commit();
            }
        }

        public async Task RefreshGitRepositoryId(Guid repositoryId)
        {
            GitRepository repository = await context.GitRepositories.FindAsync(repositoryId);

            logger.LogInformation($"Refreshing repository: {repository.GitUrl}");

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

            logger.LogInformation($"Refreshing asset dependencies for: {asset.Path}");

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

            logger.LogInformation($"Refreshing asset frameworks for: {asset.Path}");

            context.AssetFrameworks.RemoveRange(asset.Frameworks);
            await context.SaveChangesAsync();
            
            IEnumerable<Extract> items = extractionService.ExtractFrameworks(asset);

            foreach(Extract extract in items)
            {
                await AssignAssetFrameworksAsync(extract, asset);
            }
        }

        public async Task RefreshGitSourceAsync(Guid sourceId)
        {
            GitSource source = await context.GitSources.FindAsync(sourceId);
            context.GitRepositories.RemoveRange(source.GitRepositories);
            await context.SaveChangesAsync();

            IEnumerable<GitRepository> items = await gitService.GetRepositoriesAsync(source);

            foreach(GitRepository repository in items)
            {
                context.GitRepositories.Add(repository);
                await context.SaveChangesAsync();
                await RefreshGitRepositoryId(repository.Id);
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
            DependencyVersion latest = await versionService.GetLatestVersionAsync(dependency);

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

        public async Task NextRefreshTaskAsync()
        {
            RefreshTask task = await context.RefreshTasks.OrderByDescending(task => task.Created).FirstOrDefaultAsync(task => task.Status == RefreshStatus.Pending);

            if (task == null)
            {
                logger.LogInformation($"No tasks to process...");
                return;
            }

            try
            {
                task.Status = RefreshStatus.Processing;
                context.RefreshTasks.Update(task);
                await context.SaveChangesAsync();
                logger.LogInformation($"Refresh task {task.Id} processing...");

                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    switch (task.Kind)
                    {
                        case RefreshKind.Asset:
                            await RefreshAssetAsync(task.TargetId);
                            break;
                        case RefreshKind.Dependency:
                            await RefreshDependencyAsync(task.TargetId);
                            break;
                        case RefreshKind.GitRepository:
                            await RefreshGitRepositoryId(task.TargetId);
                            break;
                        case RefreshKind.GitServer:
                            await RefreshGitSourceAsync(task.TargetId);
                            break;
                    }

                    transaction.Commit();
                }
            }
            finally
            {
                task.Completed = DateTime.Now;
                task.Status = RefreshStatus.Done;

                context.RefreshTasks.Update(task);
                await context.SaveChangesAsync();
            }
        }

        public async Task RefreshDependencyAsync(Guid dependencyId)
        {
            Dependency dependency = await context.Dependencies.FindAsync(dependencyId);
            DependencyVersion latestVersion = await versionService.GetLatestVersionAsync(dependency);
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
