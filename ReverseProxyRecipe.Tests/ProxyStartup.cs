using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using ProxyKit;

namespace ReverseProxyRecipe.Tests
{
    public class ProxyStartup
    {
        public static string MappedProxyUrl = "/test-proxy-server";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddProxy(builder =>
            {
                builder.ConfigurePrimaryHttpMessageHandler(HttpClientExtensions.CreatePrimaryHandler);

                builder.ConfigureHttpClient(client => client.AddMediaTypeWithQualityHeaderValue());
            });
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(typeof(IProxyHandler)) is IProxyHandler targetProxyHandler)) return;

            app.UseXForwardedHeaders();

            app.Map(MappedProxyUrl, appInner =>
                        appInner.RunProxy(context => targetProxyHandler.HandleProxyRequest(context)));
        }
    }
}
