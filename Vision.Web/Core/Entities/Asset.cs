﻿using System;
using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class Asset : Entity
    {
        public string Path { get; set; }
        public DependencyKind Kind { get; set; }
        public string Raw { get; set; }        
        public Guid RepositoryId { get; set; }
        public virtual Repository Repository { get; set; }
        public virtual ICollection<AssetDependency> Dependencies { get; set; }
        public virtual ICollection<AssetFramework> Frameworks { get; set; }
    }
}