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

        public ProxyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IRedirectService redirectService)
        {
            var list = redirectService.GetRedirectItems();
            var path = httpContext.Request.Path.Value ?? null;

            var matchedRedirect = list.FirstOrDefault(redir =>
            {
                if (path != null && (path == redir.RedirectUrl || path.Contains(redir.RedirectUrl)))
                {
                    return true;
                }

                return false;
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
