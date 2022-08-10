using BMRedirect.Core;
using BMRedirect.Services;
using Microsoft.AspNetCore.Mvc;

namespace BMRedirect.Controllers;

[ApiController]
[Route("/")]
public class RedirectController : ControllerBase
{
    private readonly ILogger<RedirectController> _logger;
    private readonly IRedirectService _redirectService;

    public RedirectController(ILogger<RedirectController> logger, IRedirectService redirectService)
    {
        _logger = logger;
        _redirectService = redirectService;
    }

    [HttpGet(Name = "GetRedirect")]
    public async Task<IActionResult> Get()
    {
        var redirectList = await _redirectService.GetRedirectItemsAsync();
        return Ok(redirectList);
    }

    [HttpGet("campaigns/targetcampaign")]
    public IActionResult TargetCampaignA()
    {
        _logger.LogInformation($"API Endpoint Matched - campaigns/targetcampaign");
        return Ok("Redirected");
    }

    [HttpGet("campaigns/targetcampaign/channelB")]
    public IActionResult TargetCampaignB()
    {
        _logger.LogInformation($"API Endpoint Matched - campaigns/targetcampaign/channelB");
        return Ok("Redirected");
    }

    [HttpGet("products/{category}/{type}/{productId}")]
    public IActionResult Products(string category, string type, string productId)
    {
        _logger.LogInformation($"API Endpoint Matched - products/{category}/{type}/{productId}");
        return Ok("Redirected");
    }

    [HttpGet("error/404")]
    public IActionResult ErrorDefault()
    {
        _logger.LogError("API Endpoint Not Matched, 404 Returned");
        return NotFound();
    }

}


