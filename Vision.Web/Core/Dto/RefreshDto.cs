namespace Vision.Web.Core
{
    using System;

    public class SystemTaskDto
    {
        public Guid RefreshTaskId { get; set; }
        public TaskScopeKind Kind { get; set; }
        public DateTime? Started { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        public Guid TargetId { get; set; }
    }
}
