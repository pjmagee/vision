using System.Collections.Generic;

namespace Vision.Core
{
    public class Vcs : Entity
    {
        public VcsKind Kind { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public bool IsEnabled { get; set; }
        public virtual ICollection<VcsRepository> Repositories { get; set; }

        public static IEnumerable<Vcs> MockData()
        {
            yield return new Vcs
            {
                ApiKey = "",
                IsEnabled = true,
                Kind = VcsKind.Bitbucket,
                Endpoint = "http://stash.xpa.rbxd.ds:8090"
            };
        }
    }
}
