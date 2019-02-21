using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public interface IVersionService
    {
        DependencyKind Kind { get;  }
        Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency);
    }

    public class AggregateVersionService : IVersionService
    {
        public DependencyKind Kind { get; }

        public Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency)
        {
            throw new NotImplementedException();
        }
    }
}
