using Microsoft.AspNetCore.Mvc;

namespace BMRedirect.Api.Controllers
{
    [Route("[controller]")]
    public class CampaignsController : Controller
    {
        private readonly ILogger<CampaignsController> _logger;

        public CampaignsController(ILogger<CampaignsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("targetcampaign")]
        [HttpGet("targetcampaign/{subCampaign}")]
        public IActionResult TargetCampaign(string subCampaign)
        {
            _logger.LogInformation($"API Endpoint Matched - {Request.Path}");
            return Ok($"Redirected to {Request.Path}");
        }
    }
}
