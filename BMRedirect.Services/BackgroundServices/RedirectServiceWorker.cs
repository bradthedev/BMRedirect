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
        private readonly ILogger<RedirectServiceWorker> _logger;

        public RedirectServiceWorker(IRedirectService redirectService, IOptionsMonitor<CacheOptions> optionsMonitor, ILogger<RedirectServiceWorker> logger)
        {
            _redirectService = redirectService;
            _cacheOptions = optionsMonitor.CurrentValue;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var populateTask = _redirectService.PopulateCacheAsync();
                    var cancelTask = Task.Delay(_cacheOptions.RefreshInterval * 1000, stoppingToken);

                    //double await so exceptions from either task will bubble up
                    await await Task.WhenAny(populateTask, cancelTask);

                    if (!populateTask.IsCompletedSuccessfully)
                    {
                        _logger.LogError("Could Not Refresh Cache");
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                await Task.Delay(_cacheOptions.RefreshInterval * 1000, stoppingToken);
            }
        }
    }
}
