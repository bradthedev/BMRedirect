using BMRedirect.Core;
using Microsoft.Extensions.Caching.Memory;

namespace BMRedirect.Services;
public class RedirectService : IRedirectService
{
    private readonly IMemoryCache _memoryCache;

    public RedirectService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// This method get's the "manifest" of redirect items to use for our base controller
    /// </summary>
    public async Task<List<RedirectItem>> GetRedirectItemsAsync()
    {
        var cachedValue = await _memoryCache.GetOrCreateAsync("RedirectCache", cacheEntry =>
        {
            cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(30);
            return Task.FromResult(this.getRedirectItems());
        });

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
}

