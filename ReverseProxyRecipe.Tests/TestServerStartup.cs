using System.Linq;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace ReverseProxyRecipe.Tests
{
    public class TestServerStartup
    {
        private readonly ProxyContext _proxyContext;

        public TestServerStartup(ProxyContext proxyContext)
        {
            _proxyContext = proxyContext;
        }

        public void ConfigureServices(IServiceCollection services) { }

        public void Configure(IApplicationBuilder app)
        {
            var options = new ForwardedHeadersOptions();
            options.AllowedHosts.Add("*");
            options.ForwardedHeaders = ForwardedHeaders.All;
            app.UseXForwardedHeaders(options);

            app.MapWhen(context => true, a => a.Run(async ctx =>
            {
                ctx.Response.StatusCode = (int)_proxyContext.StatusCode;

                var response = new ProxyResponse
                {
                    Content = _proxyContext.Content,
                    RequestHeaders = ctx.Request.Headers.ToDictionary(pair => pair.Key, pair => pair.Value.FirstOrDefault())
                };
                await ctx.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }));
        }

        public static IWebHostBuilder KestrelBasedServerOnRandomPort()
        {
            return new WebHostBuilder()
                   .UseKestrel()
                   .UseUrls("http://*:0")
                   .UseStartup<TestServerStartup>();

        }
    }
}
