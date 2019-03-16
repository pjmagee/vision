using System;
namespace Vision.Web.Core
{
    public class SystemTask : Entity
    {
        public TaskScopeKind Scope { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Started { get; set; }
        public DateTime? Completed { get; set; }
        public Guid TargetId { get; set; }
    }
}
