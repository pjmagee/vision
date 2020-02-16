﻿using System;

namespace Vision.Web.Core
{
    public class EcosystemVersion : Entity
    {
        public string Version { get; set; }
        public Guid EcosystemId { get; set; }
        public virtual Ecosystem Ecosystem { get; set; }
    }
}
