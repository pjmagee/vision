using System;

namespace Vision.Shared
{
    public class RefreshDto
    {
        public Guid RefreshTaskId { get; set; }
        public RefreshKind Kind { get; set; }
        public RefreshStatus Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        public Guid TargetId { get; set; }
    }
}
