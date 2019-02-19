﻿using System;

namespace Vision.Shared.Api
{
    public class AssetDependencyDto
    {
        public string Name { get; set; }
        public string Version { get; set; }        
        public bool IsLatest { get; set; }
        public Guid AssetId { get; set; }
        public Guid DependencyId { get; set; }
        public Guid DependencyVersionId { get; set; }       
    }
}
