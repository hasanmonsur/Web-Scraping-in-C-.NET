namespace WebScrapingAPI.Models
{
    public class ScrapedData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Links { get; set; } = new List<string>();
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        public string RawContent { get; set; }
    }
}
