namespace Vision.Web.Core
{
    public class AlertDto
    {
        public MetricAlertKind Kind { get; set; }

        public string Message { get; set; }

        public AlertDto()
        {

        }
    }
}
