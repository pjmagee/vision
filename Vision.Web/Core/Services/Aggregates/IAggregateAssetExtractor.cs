﻿using System.Collections.Generic;

namespace Vision.Web.Core
{
    public interface IAggregateAssetExtractor
    {
        IEnumerable<Extract> ExtractDependencies(Asset asset);
        IEnumerable<Extract> ExtractFrameworks(Asset asset);
        string ExtractPublishName(Asset asset);
    }
}