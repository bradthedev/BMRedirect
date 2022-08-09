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
        var redirectList = _redirectService.GetRedirectItemsAsync();
        return Ok(redirectList);
    }
}

