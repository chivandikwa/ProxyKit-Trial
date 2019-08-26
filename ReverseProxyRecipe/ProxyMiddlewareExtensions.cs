using Microsoft.AspNetCore.Builder;

using ProxyKit;

namespace ReverseProxyRecipe
{
    public static class ProxyMiddlewareExtensions
    {
        public static void UseProxyForwarding<T>(this IApplicationBuilder applicationBuilder, string interceptPathSegment) where T : IProxyHandler
        {
            applicationBuilder.UseWhen(
                context => context.Request.Path.StartsWithSegments(interceptPathSegment),
                appInner => appInner.RunProxy<T>());
        }
    }
}