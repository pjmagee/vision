﻿using System;

namespace Vision.Shared.Api
{
    public class RepositoryDto
    {
        public Guid RepositoryId { get; set; }
        public string GitUrl { get; set; }
        public string WebUrl { get; set; }
        public int Assets { get; set;  }
    }
}
