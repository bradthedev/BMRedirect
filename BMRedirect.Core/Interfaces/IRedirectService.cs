using System;
namespace BMRedirect.Core
{
    public interface IRedirectService
    {
        Task<List<RedirectItem>> GetRedirectItemsAsync();
        Task<List<RedirectItem>> PopulateCache();
    }
}

