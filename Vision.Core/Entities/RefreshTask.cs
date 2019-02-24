using System;
using Vision.Shared;

namespace Vision.Core
{
    public class SystemTask : Entity
    {
        public TaskScope Scope { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        public Guid TargetId { get; set; }

        public SystemTask()
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
        }
    }
}
