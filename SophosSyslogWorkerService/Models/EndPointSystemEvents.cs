namespace SophosSyslogWorkerService.Models
{
    internal class EndPointSystemEvents
    {
        public bool has_more { get; set; }
        public List<Item> items { get; set; }
        public string? next_cursor { get; set; }
    }

    public class Item
    {
        public string? type { get; set; }
        public string? severity { get; set; }
        public string? created_at { get; set; }
        public SourceInfo source_info { get; set; }
        public string? customer_id { get; set; }
        public string? endpoint_id { get; set; }
        public string? endpoint_type { get; set; }
        public string? user_id { get; set; }
        public string? when { get; set; }
        public string? name { get; set; }
        public string? location { get; set; }
        public string? id { get; set; }
        public string? source { get; set; }
        public string? group { get; set; }
    }

    public class SourceInfo
    {
        public string? ip { get; set; }
    }
}

