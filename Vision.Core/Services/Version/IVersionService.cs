using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public interface IVersionService
    {
        DependencyKind Kind { get;  }
        Task<DependencyVersion> GetLatestVersion(Dependency dependency);
    }
}
