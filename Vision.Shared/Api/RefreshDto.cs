using System;

namespace Vision.Shared
{
    public class RefreshDto
    {
        public Guid RefreshTaskId { get; set; }
        public TaskScope Kind { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        public Guid TargetId { get; set; }
    }
}
