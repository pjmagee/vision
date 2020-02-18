using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vision.Core;

namespace Vision.Web.Api.Controllers
{
    [ApiController]
    public class RepositoriesControlsController : ControllerBase
    {
        private readonly ILogger<RepositoriesControlsController> logger;
        private readonly IRepositoryService repositoryService;
        private readonly IAssetService assetService;

        private const string search = null;

        public RepositoriesControlsController(
            ILogger<RepositoriesControlsController> logger,
            IRepositoryService repositoryService,
            IAssetService assetService)
        {
            this.logger = logger;
            this.repositoryService = repositoryService;
            this.assetService = assetService;
        }

        [HttpGet("/repositories/{repositoryId}")]
        public async Task<RepositoryDto> GetRepositoryById(Guid repositoryId)
        {
            return await repositoryService.GetByIdAsync(repositoryId);
        }

        [HttpGet("/repositories/{repositoryId}/assets")]
        public async Task<IEnumerable<AssetDto>> GetAssetsByRepositoryId(Guid repositoryId, int pageIndex = 1, int pageSize = 10)
        {
            return await assetService.GetByRepositoryIdAsync(repositoryId, search, Enumerable.Empty<EcosystemKind>(), dependents: false, pageIndex, pageSize);
        }
    }
}
