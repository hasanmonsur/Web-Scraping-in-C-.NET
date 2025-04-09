using WebScrapingAPI.Models;

namespace WebScrapingAPI.Services
{
    public interface IScrapingService
    {
        Task<ScrapedData> ScrapeWebsiteAsync(string url);
        Task<ScrapedData> ScrapeWebsiteWithCustomSelectorsAsync(string url, Dictionary<string, string> selectors);
    }
}
