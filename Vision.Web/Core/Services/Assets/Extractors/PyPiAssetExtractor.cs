using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class PyPiAssetExtractor : IAssetExtractor
    {
        private readonly ILogger<PyPiAssetExtractor> logger;

        public bool Supports(DependencyKind kind) => kind == DependencyKind.PyPi;

        public PyPiAssetExtractor(ILogger<PyPiAssetExtractor> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<Extract> ExtractDependencies(Asset asset)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Extract> ExtractFrameworks(Asset asset)
        {
            yield break; // Python Version ??
        }

        public string ExtractPublishName(Asset asset)
        {
            throw new NotImplementedException();
        }
    }
}
