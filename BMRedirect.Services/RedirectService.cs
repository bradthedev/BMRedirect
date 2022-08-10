using BMRedirect.Api.Configurations;
using BMRedirect.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BMRedirect.Services;
public class RedirectService : IRedirectService
{
    private const string CacheKey = "RefreshCacheKey";

    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<RedirectService> _logger;
    private readonly ILogger<RedirectService> logger;
    private readonly CacheOptions _cacheOptions;

    public RedirectService(IMemoryCache memoryCache, IOptionsMonitor<CacheOptions> optionsMonitor, ILogger<RedirectService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _cacheOptions = optionsMonitor.CurrentValue;

        Task.Run(() => StartAutoRefresh());
        Task.Run(() => StartAutoRefreshTest());
    }

    /// <summary>
    /// This method get's the "manifest" of redirect items to use for our base controller
    /// </summary>
    public async Task<List<RedirectItem>> GetRedirectItemsAsync()
    {
        var cachedValue = await this.PopulateCache();

        return cachedValue;
    }

    private List<RedirectItem> getRedirectItems()
    {
        //Build example array
        var items = new List<RedirectItem>() {
            new RedirectItem
            {
               RedirectUrl = "/campaignA",
               TargetUrl = "/campaigns/targetcampaign",
               RedirectType = 302,
               UseRelative = false
            },
            new RedirectItem
            {
               RedirectUrl = "/campaignB",
               TargetUrl = "/campaigns/targetcampaign/channelB",
               RedirectType = 302,
               UseRelative = false
            },
            new RedirectItem
            {
               RedirectUrl = "/product-directory",
               TargetUrl = "/products",
               RedirectType = 301,
               UseRelative = true
            }
        };

        return items;
    }

    private async Task<List<RedirectItem>> PopulateCache()
    {
        return await _memoryCache.GetOrCreateAsync(CacheKey, cacheEntry =>
        {
            _logger.LogInformation($"Refreshing Cache for {CacheKey} on Thread {Thread.CurrentThread.ManagedThreadId}");
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_cacheOptions.RefreshInterval);
            cacheEntry.RegisterPostEvictionCallback(LogEviction);
            return Task.FromResult(this.getRedirectItems());
        });
    }

    private async Task StartAutoRefresh()
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(_cacheOptions.RefreshInterval));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            _ = PopulateCache();
        }
    }

    private async Task StartAutoRefreshTest()
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(_cacheOptions.RefreshInterval));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            var x = await PopulateCache();
            if (x != null)
            {
                //Console.WriteLine($"Grabbing Cache for {CacheKey} on Thread {Thread.CurrentThread.ManagedThreadId}");
            }

        }
    }

    private void LogEviction(object key, object value, EvictionReason reason, object state)
    {
        Console.WriteLine($"'{key}':'{value}' was evicted because: {reason}");
    }
}

