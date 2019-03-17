﻿namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class DependenciesService
    {
        private readonly VisionDbContext context;
        private readonly RepositoryAssetsService repositoryAssetsService;

        public DependenciesService(VisionDbContext context, RepositoryAssetsService repositoryAssetsService)
        {            
            this.context = context;
            this.repositoryAssetsService = repositoryAssetsService;
        }

        public async Task<PaginatedList<DependencyDto>> GetAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Dependencies.Select(dependency => new DependencyDto
            {
                DependencyId = dependency.Id,
                Name = dependency.Name,
                Kind = dependency.Kind,
                Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id),
                Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id),
                RepositoryUrl = dependency.RepositoryUrl
            });

            return await PaginatedList<DependencyDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<PaginatedList<DependencyDto>> GetByRepositoryIdAsync(Guid repositoryId, int pageIndex = 1, int pageSize = 10)
        {
            // Find all Dependencies who's Project URl (META DATA FROM DEPENDENCY REGISTRY) matches this Repositories Git or Web Url
            // TODO: Ensure all XpertHR Assets which create dependencies (nuspec files) have the repository in the nuspec <RepositoryUrl></RepositoryUrl>

            Repository repository = await context.Repositories.FindAsync(repositoryId);
            List<string> assetNames = await repositoryAssetsService.GetPublishedNamesByRepositoryIdAsync(repositoryId);

            IQueryable<DependencyDto> query = context.Dependencies.Where(dependency => assetNames.Contains(dependency.Name) || string.Equals(dependency.RepositoryUrl, repository.Url) || string.Equals(dependency.RepositoryUrl, repository.WebUrl)).Select(dependency => new DependencyDto
            {
                Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id),
                DependencyId = dependency.Id,
                Kind = dependency.Kind,
                Name = dependency.Name,
                RepositoryUrl = dependency.RepositoryUrl,
                Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id)
            })
            .OrderByDescending(d => d.Versions)
            .ThenByDescending(d => d.Assets);

            return await PaginatedList<DependencyDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<PaginatedList<DependencyDto>> GetByRegistryIdAsync(Guid registryId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Dependencies
                 .Where(d => d.RegistryId == registryId)
                 .Select(entity => new DependencyDto
                 {
                     Name = entity.Name,
                     RepositoryUrl = entity.RepositoryUrl,
                     Versions = context.DependencyVersions.Count(x => x.DependencyId == entity.Id),
                     Assets = context.AssetDependencies.Count(x => x.DependencyId == entity.Id),
                     DependencyId = entity.Id,
                     Kind = entity.Kind
                 })
                 .OrderByDescending(d => d.Versions)
                 .ThenByDescending(d => d.Assets);

            return await PaginatedList<DependencyDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<PaginatedList<DependencyDto>> GetByKindsAsync(DependencyKind[] kinds, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Dependencies.Where(dependency => kinds.Contains(dependency.Kind)).Select(dependency => new DependencyDto
            {
                DependencyId = dependency.Id,
                Name = dependency.Name,
                Kind = dependency.Kind,
                Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id),
                Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id),
                RepositoryUrl = dependency.RepositoryUrl
            })
            .OrderByDescending(a => a.Versions)
            .ThenByDescending(d => d.Assets);

            return await PaginatedList<DependencyDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<DependencyDto> GetByIdAsync(Guid dependencyId)
        {
            Dependency dependency = await context.Dependencies.FindAsync(dependencyId);

            return new DependencyDto
            {
                Name = dependency.Name,
                Kind = dependency.Kind,
                DependencyId = dependency.Id,
                RepositoryUrl = dependency.RepositoryUrl,
                Assets = await context.AssetDependencies.CountAsync(assetDependency => assetDependency.DependencyId == dependencyId),
                Versions = await context.DependencyVersions.CountAsync(dependencyVersion => dependencyVersion.DependencyId == dependencyId),
            };
        }       
    }
}
