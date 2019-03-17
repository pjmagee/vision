namespace Vision.Web.Core
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RepositoryAssetsService
    {
        private readonly IEnumerable<IAssetExtractor> extractors;
        private readonly VisionDbContext context;

        public RepositoryAssetsService(VisionDbContext context, NuGetAssetExtractor nugetNameExtractor, NpmAssetExtractor npmNameExtractor)
        {
            this.context = context;
            extractors = new IAssetExtractor[] { nugetNameExtractor, npmNameExtractor };
        }

        public async Task<List<string>> GetPublishedNamesByRepositoryIdAsync(Guid repositoryId)
        {
            var assets = await context.Assets.Where(asset => asset.RepositoryId == repositoryId).ToListAsync();
            return extractors.SelectMany(extractor => assets.Where(asset => extractor.Supports(asset.Kind)).Select(asset => extractor.ExtractPublishName(asset))).ToList();
        }
    }
}
