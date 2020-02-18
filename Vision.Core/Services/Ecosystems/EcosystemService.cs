using System;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Core
{
    public class EcosystemService : IEcosystemService
    {
        private readonly VisionDbContext context;

        public EcosystemService(VisionDbContext context) => this.context = context;

        public async Task<IPaginatedList<EcosystemDto>> GetAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Ecosystems.Select(ecosystem => new EcosystemDto
            {
                Name = ecosystem.Name,
                Versions = context.EcosystemVersions.Count(x => x.EcosystemId == ecosystem.Id),
                EcosystemId = ecosystem.Id,
                Assets = context.Assets.Count(asset => asset.AssetEcosystem.EcosystemVersion.EcosystemId == ecosystem.Id)
            })
            .OrderByDescending(f => f.Assets);

            return await PaginatedList<EcosystemDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<EcosystemDto> GetEcosystemByIdAsync(Guid ecoId)
        {
            Ecosystem ecosystem = await context.Ecosystems.FindAsync(ecoId);

            return new EcosystemDto
            {
                Name = ecosystem.Name,
                Versions = context.EcosystemVersions.Count(x => x.EcosystemId == ecosystem.Id),
                EcosystemId = ecosystem.Id,
                Assets = context.Assets.Count(asset => asset.AssetEcosystem.EcosystemVersion.EcosystemId == ecosystem.Id)
            };
        }

        public async Task<EcosystemVersionDto> GetEcoVersionByIdAsync(Guid ecoVerId)
        {
            EcosystemVersion version = await context.EcosystemVersions.FindAsync(ecoVerId);

            return new EcosystemVersionDto
            {
                Version = version.Version,
                EcosystemVersionId = ecoVerId,
                EcosystemId = version.Id,
                Assets = context.Assets.Count(asset => asset.AssetEcosystem.EcosystemVersionId == version.Id)
            };
        }

        public async Task<IPaginatedList<EcosystemDto>> GetByAssetIdAsync(Guid assetId, int pageIndex = 1, int pageSize = 10)
        {
            var ecosystems = context.Ecosystems.Where(ecosystem => context.AssetEcoSystems.Any(ar => ar.AssetId == assetId && ar.EcosystemVersion.EcosystemId == ecosystem.Id));

            var query = ecosystems.Select(ecosystem => new EcosystemDto
            {
                EcosystemId = ecosystem.Id,
                Name = ecosystem.Name,
                Versions = context.EcosystemVersions.Count(x => x.EcosystemId == ecosystem.Id),
                Assets = context.Assets.Count(asset => asset.AssetEcosystem.EcosystemVersion.EcosystemId == ecosystem.Id)
            })
            .OrderByDescending(rt => rt.Assets);

            return await PaginatedList<EcosystemDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<EcosystemDto>> GetByRepositoryIdAsync(Guid repoId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Ecosystems
                .Where(ecosystem => context.AssetEcoSystems.Any(assetEco => assetEco.EcosystemVersion.EcosystemId == ecosystem.Id && context.Assets.Any(a => a.RepositoryId == repoId && assetEco.AssetId == a.Id)))
                .Select(ecosystem => new EcosystemDto
                {
                    Assets = context.Assets.Where(asset => asset.RepositoryId == repoId).Count(asset => asset.AssetEcosystem.EcosystemVersion.EcosystemId == ecosystem.Id),
                    EcosystemId = ecosystem.Id,
                    Name = ecosystem.Name,
                    Versions = context.EcosystemVersions.Count(rtv => rtv.EcosystemId == ecosystem.Id)
                })
                .OrderByDescending(rt => rt.Assets);

            return await PaginatedList<EcosystemDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<EcosystemVersionDto>> GetByEcosystemIdAsync(Guid ecoId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.EcosystemVersions
                .Where(ecoVersion => ecoVersion.EcosystemId == ecoId)
                .Select(ecoVersion => new EcosystemVersionDto
                {
                    Assets = context.Assets.Count(asset => asset.AssetEcosystem.EcosystemVersionId == ecoVersion.Id),
                    EcosystemId = ecoId,
                    EcosystemVersionId = ecoVersion.Id,
                    Version = ecoVersion.Version
                })
                .OrderByDescending(rtv => rtv.Assets);

            return await PaginatedList<EcosystemVersionDto>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
