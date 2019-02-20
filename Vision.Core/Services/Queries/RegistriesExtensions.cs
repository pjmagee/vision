using System.Linq;
using Vision.Shared;

namespace Vision.Core.Services.Queries
{
    public static class RegistriesExtensions
    {
        public static IQueryable<Registry> FilterByKind(this IQueryable<Registry> registries, DependencyKind kind)
        {
            switch (kind)
            { 
                case DependencyKind.Docker: registries = registries.Where(x => x.IsDocker); break;
                case DependencyKind.NuGet: registries = registries.Where(x => x.IsNuGet); break;
                case DependencyKind.Maven: registries = registries.Where(x => x.IsMaven); break;
                case DependencyKind.Npm: registries = registries.Where(x => x.IsNpm); break;
                case DependencyKind.PyPi: registries = registries.Where(x => x.IsPyPi); break;
                case DependencyKind.RubyGem: registries = registries.Where(x => x.IsRubyGem); break;
            }

            return registries;
        }
    }
}
