using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMRedirect.Core.Middleware
{
    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;

        public ProxyMiddleware(RequestDelegate next) => _next = next ?? throw new ArgumentNullException(nameof(next));

        public async Task InvokeAsync(HttpContext httpContext, IRedirectService redirectService)
        {
            var list = await redirectService.GetRedirectItemsAsync();
            var path = httpContext.Request.Path.Value ?? null;

            var matchedRedirect = list.FirstOrDefault(redir =>
            {
                return path != null && (path == redir.RedirectUrl || path.Contains(redir.RedirectUrl));
            });

            if (path != null && matchedRedirect != null)
            {
                var newPath =
                    matchedRedirect.UseRelative
                        ? path.Replace(matchedRedirect.RedirectUrl, matchedRedirect.TargetUrl)
                        : matchedRedirect.TargetUrl;

                httpContext.Response.Redirect(newPath, matchedRedirect.UseRelative);

                return;
            }

            await _next(httpContext);
        }
    }

    public static class ProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseProxyMiddlware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ProxyMiddleware>();
        }
    }
}
