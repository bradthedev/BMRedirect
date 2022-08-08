using BMRedirect.Core;

namespace BMRedirect.Services;
public class RedirectService : IRedirectService
{
    /// <summary>
    /// This method get's the "manifest" of redirect items to use for our base controller
    /// </summary>
    public List<RedirectItem> GetRedirectItems()
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

