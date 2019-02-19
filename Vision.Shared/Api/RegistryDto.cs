using System;
using System.Collections.Generic;

namespace Vision.Shared.Api
{
    public class RegistryDto
    {
        public Guid RegistryId { get; set;  }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public bool IsPublic { get; set; }
        public bool IsDocker { get; set; }
        public bool IsNuGet { get; set; }
        public bool IsMaven { get; set; }
        public bool IsPyPi { get; set; }
        public bool IsRubygem { get; set; }
        public bool IsNpm { get; set; }
        public int Dependencies { get; set; }     
        
        public IEnumerable<DependencyKind> Kinds
        {
            get
            {
                if (IsDocker) yield return DependencyKind.Docker;
                if (IsNuGet) yield return DependencyKind.NuGet;
                if (IsMaven) yield return DependencyKind.Maven;
                if (IsPyPi) yield return DependencyKind.PyPi;
                if (IsRubygem) yield return DependencyKind.RubyGem;
                if (IsNpm) yield return DependencyKind.Npm;
            }
        }
    }
}
