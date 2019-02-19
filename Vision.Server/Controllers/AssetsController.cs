using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vision.Core;
using Vision.Shared;

namespace Vision.Server.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetRepository assetsRepository;

        public AssetsController(IAssetRepository assetsRepository)
        {
            this.assetsRepository = assetsRepository;
        }
        
        [HttpGet("{repositoryId}")]
        public async Task<IEnumerable<AssetDto>> GetAsync(Guid repositoryId)
        {
            IEnumerable<Asset> assets = await assetsRepository.GetByRepositoryIdAsync(repositoryId);
            return assets.Select(a => new AssetDto { AssetId = a.Id, Dependencies = a.Dependencies.Count, Path = a.Path, RepositoryId = a.GitRepositoryId });
        }
    }
}
