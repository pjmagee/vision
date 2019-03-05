using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vision.Core;
using Vision.Shared;

namespace Vision.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ResponseCache(Duration = 30)]
    [Produces("application/json")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class InsightsController : Controller
    {
        private readonly VisionDbContext context;

        public InsightsController(VisionDbContext context) => this.context = context;

        [HttpGet("assets/{dependencyKind:int}")]
        public async Task<IEnumerable<MetricItems<AssetDto>>> GetAssetInsightsByKindAsync(DependencyKind dependencyKind)
        {
            IQueryable<Asset> assets = context.Assets.Where(asset => asset.Kind == dependencyKind);

            AssetDto[] most = await assets
                .OrderByDescending(asset => context.AssetDependencies.Count(ad => ad.AssetId == asset.Id))
                .Take(10)
                .Select(asset => new AssetDto { AssetId = asset.Id, Repository = asset.Repository.Url, Asset = asset.Path, Dependencies = context.AssetDependencies.Count(ad => ad.AssetId == asset.Id) })
                .ToArrayAsync();

            AssetDto[] least = await assets
                .OrderBy(asset => context.AssetDependencies.Count(ad => ad.AssetId == asset.Id))
                .Take(10)
                .Select(asset => new AssetDto { AssetId = asset.Id, Repository = asset.Repository.Url, Asset = asset.Path, Dependencies = context.AssetDependencies.Count(ad => ad.AssetId == asset.Id) })
                .ToArrayAsync();

            return new[]
            {
                new MetricItems<AssetDto>(MetricAlertKind.Standard, MetricCategoryKind.Assets, $"{dependencyKind} assets with the most dependencies", most),
                new MetricItems<AssetDto>(MetricAlertKind.Standard, MetricCategoryKind.Assets, $"{dependencyKind} assets with the least dependencies", least)
            };
        }

        [HttpGet("repositories/{dependencyKind:int}")]
        public async Task<IEnumerable<MetricItems<RepositoryDto>>> GetPublishingRepositoriesByDependencyKind(DependencyKind dependencyKind)
        {
            RepositoryDto[] published = await context.Repositories.AsNoTracking()
                .Where(repository => context.Dependencies.Any(dependency => dependency.Kind == dependencyKind && (string.Equals(dependency.RepositoryUrl, dependency.RepositoryUrl) || string.Equals(dependency.RepositoryUrl, repository.WebUrl))))
                .Select(repository => new RepositoryDto { Assets = context.Assets.Count(a => a.RepositoryId == repository.Id), RepositoryId = repository.Id, Url = repository.Url, WebUrl = repository.WebUrl, VersionControlId = repository.VersionControlId })
                .ToArrayAsync();

            return new[]
            {
                new MetricItems<RepositoryDto>(MetricAlertKind.Standard, MetricCategoryKind.Dependencies, $"Repositories that publish {dependencyKind} dependencies", published)
            };
        }

        [HttpGet("dependencies/{dependencyKind:int}")]
        public async Task<IEnumerable<MetricItems<DependencyDto>>> GetDependencyInsightsByKindAsync(DependencyKind dependencyKind)
        {
            IQueryable<Dependency> dependencies = context.Dependencies.Where(dependency => dependency.Kind == dependencyKind);

            DependencyDto[] most = await dependencies
                .OrderByDescending(dependency => context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id)).Take(10)
                .Select(dependency => new DependencyDto { DependencyId = dependency.Id, Name = dependency.Name, Kind = dependency.Kind, Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id), Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id) })
                .ToArrayAsync();

            DependencyDto[] least = await dependencies
                .OrderBy(asset => context.AssetDependencies.Count(ad => ad.AssetId == asset.Id)).Take(10)
                .Select(dependency => new DependencyDto { DependencyId = dependency.Id, Name = dependency.Name, Kind = dependency.Kind, Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id), Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id) })
                .ToArrayAsync();

            return new[]
            {
                new MetricItems<DependencyDto>(MetricAlertKind.Standard, MetricCategoryKind.Dependencies, $"{dependencyKind} dependencies with the most assets", most),
                new MetricItems<DependencyDto>(MetricAlertKind.Standard, MetricCategoryKind.Dependencies, $"{dependencyKind} dependencies with the least assets", least)
            };
        }
    }
}
