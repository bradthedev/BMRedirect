using System;
namespace BMRedirect.Core
{
    public interface IRedirectService
    {
        List<RedirectItem> GetRedirectItems();
    }
}

