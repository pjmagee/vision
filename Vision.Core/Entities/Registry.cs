using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Vision.Shared;

namespace Vision.Core
{
    public class Registry : Entity
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public bool IsPublic { get; set; }    
        public bool IsDocker { get; set; }
        public bool IsNuGet { get; set; }
        public bool IsMaven { get; set; }
        public bool IsPyPi { get; set; }
        public bool IsRubyGem { get; set; }
        public bool IsNpm { get; set; }
        public bool IsEnabled { get; set; }
        
        public List<Dependency> Dependencies { get; set; }

        [NotMapped]
        public IEnumerable<DependencyKind> Kinds
        {
            get
            {
                if (IsDocker) yield return DependencyKind.Docker;
                if (IsNuGet) yield return DependencyKind.NuGet;
                if (IsMaven) yield return DependencyKind.Maven;
                if (IsPyPi) yield return DependencyKind.PyPi;
                if (IsRubyGem) yield return DependencyKind.RubyGem;
                if (IsNpm) yield return DependencyKind.Npm;
            }
        }
    }
}
