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
    public IEnumerable<RedirectItem> Get()
    {
        return _redirectService.GetRedirectItems();
    }

    //[HttpGet(Name = "GetWeatherForecast")]
    //public IEnumerable<WeatherForecast> Get()
    //{
    //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //    {
    //        Date = DateTime.Now.AddDays(index),
    //        TemperatureC = Random.Shared.Next(-20, 55),
    //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //    })
    //    .ToArray();
    //}
}

