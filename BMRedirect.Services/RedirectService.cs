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
    /// Gets list of Redirect Items and their Mappings
    /// </summary>
    public async Task<List<RedirectItem>> GetRedirectItemsAsync()
    {
        var cachedValue = await PopulateCacheAsync();

        return cachedValue;
    }

    /// <summary>
    /// Example method that is a mock of the Redirect Item API
    /// </summary>
    private List<RedirectItem> GetRedirectItemsList()
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

    /// <summary>
    /// Method to handle caching of the Refresh Data
    /// </summary>
    /// <remakrs>
    /// This method will get the data from the cache for our Redirect Service. 
    /// If there is not data in the Redirect Service Cache then the data will either be created or 
    /// refreshed based on the rules provided in settings for refresh time.
    /// </remakrs>
    public async Task<List<RedirectItem>> PopulateCacheAsync()
    {
        
        return await _memoryCache.GetOrCreateAsync(CacheKey, cacheEntry =>
        {
            _logger.LogInformation($"Refreshing Cache for {CacheKey} on Thread {Thread.CurrentThread.ManagedThreadId}");
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_cacheOptions.RefreshInterval);
            cacheEntry.RegisterPostEvictionCallback(LogEviction);
            return Task.FromResult(GetRedirectItemsList());
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

    /// <summary>
    /// Method to test multiple thread access 
    /// </summary>
    private async Task StartAutoRefreshTest()
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(_cacheOptions.RefreshInterval * 100));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            Task.Run(() => PopulateCacheAsync());
        }
    }

    private void LogEviction(object key, object value, EvictionReason reason, object state)
    {
        //Console.WriteLine($"'{key}':'{value}' was evicted because: {reason}");
    }
}

