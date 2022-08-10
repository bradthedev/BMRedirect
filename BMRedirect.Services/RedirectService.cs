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
    private readonly CacheOptions _cacheOptions;

    public RedirectService(IMemoryCache memoryCache, IOptionsMonitor<CacheOptions> optionsMonitor, ILogger<RedirectService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _cacheOptions = optionsMonitor.CurrentValue;
        //Task.Run(() => StartAutoRefreshTest());
    }

    /// <summary>
    /// This method get's the "manifest" of redirect items to use for our base controller
    /// </summary>
    public async Task<List<RedirectItem>> GetRedirectItemsAsync()
    {
        var cachedValue = await this.PopulateCacheAsync();

        return cachedValue;
    }

    private List<RedirectItem> GetRedirectItems()
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

    public async Task<List<RedirectItem>> PopulateCacheAsync()
    {
        return await _memoryCache.GetOrCreateAsync(CacheKey, cacheEntry =>
        {
            _logger.LogInformation($"Refreshing Cache for {CacheKey} on Thread {Thread.CurrentThread.ManagedThreadId}");
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_cacheOptions.RefreshInterval);
            cacheEntry.RegisterPostEvictionCallback(LogEviction);
            return Task.FromResult(this.GetRedirectItems());
        });
    }

    private async Task StartAutoRefresh()
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(_cacheOptions.RefreshInterval));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            _ = PopulateCacheAsync();
        }
    }

    private async Task StartAutoRefreshTest()
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(_cacheOptions.RefreshInterval * 100));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            var x = await PopulateCacheAsync();
            if (x != null)
            {
                Console.WriteLine($"Grabbing Cache for {CacheKey} on Thread {Thread.CurrentThread.ManagedThreadId}");
            }

        }
    }

    private void LogEviction(object key, object value, EvictionReason reason, object state)
    {
        //Console.WriteLine($"'{key}':'{value}' was evicted because: {reason}");
    }
}

