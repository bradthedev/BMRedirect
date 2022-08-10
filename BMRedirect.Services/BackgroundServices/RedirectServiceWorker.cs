using BMRedirect.Api.Configurations;
using BMRedirect.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMRedirect.Services.BackgroundServices
{
    public class RedirectServiceWorker : BackgroundService
    {
        private readonly IRedirectService _redirectService;
        private readonly CacheOptions _cacheOptions;
        private readonly ILogger<RedirectService> _logger;
        private const string CacheKey = "RefreshCacheKey";

        public RedirectServiceWorker(IRedirectService redirectService, IOptionsMonitor<CacheOptions> optionsMonitor, ILogger<RedirectService> logger)
        {
            _redirectService = redirectService;
            _cacheOptions = optionsMonitor.CurrentValue;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(_cacheOptions.RefreshInterval));
            while (await periodicTimer.WaitForNextTickAsync())
            {
                var x = await _redirectService.PopulateCache();
            }
        }
    }
}
