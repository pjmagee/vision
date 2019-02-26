namespace Vision.Shared
{
    public class RepositoryMetricDto
    {
        public MetricsKind Kind { get; set; }
        public string Title { get; set; }
        public RepositoryDto[] Items { get; set; }

        public RepositoryMetricDto(MetricsKind kind, string title, RepositoryDto[] items)
        {
            Kind = kind;
            Title = title;
            Items = items;
        }
    }
}
