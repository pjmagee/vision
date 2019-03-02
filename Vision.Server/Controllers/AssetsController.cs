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
    public class AssetsController : ControllerBase
    {
        private readonly VisionDbContext context;

        public AssetsController(VisionDbContext context)
        {
            this.context = context;
        }

        [Route("metrics")]
        public async Task<IEnumerable<MetricItems<AssetDto>>> GetAssetMetricsAsync()
        {
            List<MetricItems<AssetDto>> metrics = new List<MetricItems<AssetDto>>();

            foreach (DependencyKind kind in Enum.GetValues(typeof(DependencyKind)).Cast<DependencyKind>())
            {
                IQueryable<Asset> assets = context.Assets.Where(asset => asset.Kind == kind);

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

                metrics.Add(new MetricItems<AssetDto>(MetricKind.Standard, MetricCategoryKind.Assets, $"{kind} assets with the most dependencies", most));
                metrics.Add(new MetricItems<AssetDto>(MetricKind.Standard, MetricCategoryKind.Assets, $"{kind} assets with the least dependencies", least));
            }

            return metrics;
        }

        [HttpGet("{assetId}/metrics")]
        public async Task<IEnumerable<MetricItem>> GetMetricsByAssetIdAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);
            List<MetricItem> metrics = new List<MetricItem>();

            // the assets most updated
            // the assets least updated

            return metrics;
        }

        [HttpGet("{assetId}")]
        public async Task<AssetDto> GetAssetByIdAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);

            return new AssetDto
            {
                AssetId = asset.Id,
                Dependencies = await context.AssetDependencies.CountAsync(x => x.AssetId == assetId),
                Asset = asset.Path,
                RepositoryId = asset.RepositoryId
            };
        }

        [HttpGet("{assetId}/dependencies")]
        public async Task<IEnumerable<AssetDependencyDto>> GetDependenciesByAssetIdAsync(Guid assetId)
        {
            return await context.AssetDependencies.Where(assetDependency => assetDependency.AssetId == assetId).Select(assetDependency => new AssetDependencyDto
            {
                Repository = assetDependency.Asset.Repository.WebUrl,
                Dependency = assetDependency.Dependency.Name,
                Version = assetDependency.DependencyVersion.Version,
                AssetId = assetDependency.AssetId,
                Asset = assetDependency.Asset.Path,
                DependencyId = assetDependency.DependencyId,
                DependencyVersionId = assetDependency.DependencyVersionId,
                IsLatest = assetDependency.DependencyVersion.IsLatest,
                RepositoryId = assetDependency.Asset.RepositoryId
            })
            .ToListAsync();
        }

        [HttpGet("{assetId}/frameworks")]
        public async Task<IEnumerable<AssetFrameworkDto>> GetFrameworksByAssetId(Guid assetId)
        {
            return await context.AssetFrameworks
                .Where(x => x.AssetId == assetId)
                .Select(x => new AssetFrameworkDto { FrameworkId = x.FrameworkId, Name = x.Framework.Version, AssetId = x.AssetId })
                .ToListAsync();
        }

        [HttpPost("search")]
        public async Task<IEnumerable<AssetDto>> GetAssetByIdAsync([FromBody] AssetSearch assetSearch)
        {
            var dependencyIds = assetSearch.DependencyIds;
            var versionControlIds = assetSearch.VersionControlIds;

            IQueryable<Asset> query = context.Assets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(assetSearch.Name))
            {
                query = query.Where(asset => asset.Path.Contains(assetSearch.Name));
            }

            if (dependencyIds.Any())
            {
                query = query.Where(asset => versionControlIds.Contains(asset.Repository.VersionControlId));
            }

            if (versionControlIds.Any())
            {
                query = query.Where(asset => context.AssetDependencies.Any(assetDependency => assetDependency.AssetId == asset.Id && dependencyIds.Contains(assetDependency.Id)));
            }

            return await query.Select(asset => new AssetDto { AssetId = asset.Id, Dependencies = context.AssetDependencies.Count(x => x.AssetId == x.Id), Asset = asset.Path, RepositoryId = asset.RepositoryId }).ToListAsync();
        }
    }
}
