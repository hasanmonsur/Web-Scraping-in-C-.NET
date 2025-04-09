using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebScrapingAPI.Models;
using WebScrapingAPI.Services;

namespace WebScrapingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScrapingController : ControllerBase
    {
        private readonly IScrapingService _scrapingService;
        private readonly ILogger<ScrapingController> _logger;

        public ScrapingController(IScrapingService scrapingService, ILogger<ScrapingController> logger)
        {
            _scrapingService = scrapingService;
            _logger = logger;
        }

        [HttpGet("scrape")]
        public async Task<ActionResult<ScrapedData>> ScrapeWebsite([FromQuery] string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return BadRequest("URL is required");
                }

                if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                {
                    return BadRequest("Invalid URL format");
                }

                var result = await _scrapingService.ScrapeWebsiteAsync(url);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping website: {Url}", url);
                return StatusCode(500, "An error occurred while scraping the website");
            }
        }

        [HttpPost("scrape/custom")]
        public async Task<ActionResult<ScrapedData>> ScrapeWebsiteWithCustomSelectors(
            [FromQuery] string url,
            [FromBody] Dictionary<string, string> selectors)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return BadRequest("URL is required");
                }

                if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                {
                    return BadRequest("Invalid URL format");
                }

                if (selectors == null || selectors.Count == 0)
                {
                    return BadRequest("At least one selector is required");
                }

                var result = await _scrapingService.ScrapeWebsiteWithCustomSelectorsAsync(url, selectors);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping website with custom selectors: {Url}", url);
                return StatusCode(500, "An error occurred while scraping the website");
            }
        }
    }
}
