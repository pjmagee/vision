using System;
using Vision.Shared;

namespace Vision.Core
{
    public class RefreshTask : Entity
    {
        public RefreshKind Kind { get; set; }
        public RefreshStatus Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        public Guid TargetId { get; set; }

        public RefreshTask()
        {
            Created = DateTime.Now;
        }
    }
}
