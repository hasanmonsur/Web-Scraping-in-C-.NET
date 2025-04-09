using HtmlAgilityPack;
using WebScrapingAPI.Models;

namespace WebScrapingAPI.Services
{
    public class ScrapingService : IScrapingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScrapingService> _logger;

        public ScrapingService(HttpClient httpClient, ILogger<ScrapingService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ScrapedData> ScrapeWebsiteAsync(string url)
        {
            try
            {
                // Fetch the HTML content
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var htmlContent = await response.Content.ReadAsStringAsync();

                // Parse the HTML
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                // Extract data
                var result = new ScrapedData
                {
                    Title = htmlDoc.DocumentNode.SelectSingleNode("//title")?.InnerText,
                    RawContent = htmlContent
                };

                // Extract all links
                var linkNodes = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
                if (linkNodes != null)
                {
                    foreach (var link in linkNodes)
                    {
                        var href = link.GetAttributeValue("href", "");
                        if (!string.IsNullOrEmpty(href))
                        {
                            result.Links.Add(href);
                        }
                    }
                }

                // Extract meta tags
                var metaNodes = htmlDoc.DocumentNode.SelectNodes("//meta");
                if (metaNodes != null)
                {
                    foreach (var meta in metaNodes)
                    {
                        var name = meta.GetAttributeValue("name", "") ??
                                   meta.GetAttributeValue("property", "") ??
                                   meta.GetAttributeValue("itemprop", "");
                        var content = meta.GetAttributeValue("content", "");

                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(content))
                        {
                            result.Metadata[name] = content;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping website: {Url}", url);
                throw;
            }
        }

        public async Task<ScrapedData> ScrapeWebsiteWithCustomSelectorsAsync(string url, Dictionary<string, string> selectors)
        {
            try
            {
                // Fetch the HTML content
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var htmlContent = await response.Content.ReadAsStringAsync();

                // Parse the HTML
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                var result = new ScrapedData();

                foreach (var selector in selectors)
                {
                    var nodes = htmlDoc.DocumentNode.SelectNodes(selector.Value);
                    if (nodes != null)
                    {
                        var contents = nodes.Select(n => n.InnerText.Trim()).ToList();
                        result.Metadata[selector.Key] = string.Join(" | ", contents);
                    }
                }

                result.RawContent = htmlContent;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping website with custom selectors: {Url}", url);
                throw;
            }
        }
    }
}
