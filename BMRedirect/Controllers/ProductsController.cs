using Microsoft.AspNetCore.Mvc;

namespace BMRedirect.Api.Controllers
{
    [Route("[controller]")]
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{category}/{type}/{productId}")]
        public IActionResult Products(string category, string type, string productId)
        {
            _logger.LogInformation($"API Endpoint Matched - {Request.Path}");
            return Ok($"Redirected to {Request.Path}");
        }
    }
}
