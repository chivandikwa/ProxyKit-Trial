using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using ProxyKit;

using Xunit;

namespace ReverseProxyRecipe.Tests
{
    public class ProxyHandlerTests
    {
        private readonly ProxyContext _okProxyContext = new ProxyContext
        {
            Content = "sample content",
            StatusCode = HttpStatusCode.OK
        }; 

        [Fact]
        public async Task ProxyHandler_OnReceivingSuccessResponse_ForwardsBack()
        {
            using (IWebHost targetServer = CreateTargetServer(_okProxyContext))
            {
                await targetServer.StartAsync();
                int port = targetServer.GetServerPort();

                using (TestServer proxyServer = CreateProxyServer(port))
                {
                    HttpClient client = proxyServer.CreateClient();

                    HttpResponseMessage result = await client.GetAsync(ProxyStartup.MappedProxyUrl);
                    var content = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(ProxyResponse)) as ProxyResponse;

                    result.StatusCode.Should().Be(_okProxyContext.StatusCode);
                    content?.Content.Should().Be(_okProxyContext.Content);
                }
            }
        }

        [Fact]
        public async Task ProxyHandler_ShouldAddCorrelationIdIfNoneExists()
        {
            using (IWebHost targetServer = CreateTargetServer(_okProxyContext))
            {
                await targetServer.StartAsync();
                int port = targetServer.GetServerPort();

                using (TestServer proxyServer = CreateProxyServer(port))
                {
                    HttpClient client = proxyServer.CreateClient();

                    HttpResponseMessage result = await client.GetAsync(ProxyStartup.MappedProxyUrl);
                    var content = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(ProxyResponse)) as ProxyResponse;

                    (content != null && content.RequestHeaders.ContainsKey(CorrelationIdExtensions.XCorrelationId)).Should().BeTrue();
                    content.RequestHeaders[CorrelationIdExtensions.XCorrelationId].Should().NotBeNullOrWhiteSpace();
                }
            }
        }

        [Fact]
        public async Task ProxyHandler_ShouldForwardCorrelationIdIfOneExists()
        {
            string correlationId = Guid.NewGuid().ToString();

            using (IWebHost targetServer = CreateTargetServer(_okProxyContext))
            {
                await targetServer.StartAsync();
                int port = targetServer.GetServerPort();

                using (TestServer proxyServer = CreateProxyServer(port))
                {
                    HttpClient client = proxyServer.CreateClient();
                    client.DefaultRequestHeaders.Add(CorrelationIdExtensions.XCorrelationId, correlationId);

                    HttpResponseMessage result = await client.GetAsync(ProxyStartup.MappedProxyUrl);
                    var content = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync(), typeof(ProxyResponse)) as ProxyResponse;

                    content?.RequestHeaders.ContainsKey(CorrelationIdExtensions.XCorrelationId).Should().BeTrue();
                    content?.RequestHeaders[CorrelationIdExtensions.XCorrelationId].Should().Be(correlationId);
                }
            }
        }

        [Fact]
        public async Task ProxyHandler_ShouldReturnServiceUnavailableStatusWhenTargetServerIsDown()
        {
            using (IWebHost targetServer = CreateTargetServer(_okProxyContext))
            {
                await targetServer.StartAsync();

                int port = targetServer.GetServerPort();

                await targetServer.StopAsync();

                using (TestServer proxyServer = CreateProxyServer(port))
                {
                    HttpClient client = proxyServer.CreateClient();

                    HttpResponseMessage result = await client.GetAsync(ProxyStartup.MappedProxyUrl);

                    result.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
                }
            }
        }

        private static IWebHost CreateTargetServer(ProxyContext context)
        {
            return TestServerStartup.KestrelBasedServerOnRandomPort()
                                    .ConfigureServices(collection =>
                                                           collection.AddSingleton(context))
                                    .Build();
        }

        private static TestServer CreateProxyServer(int port)
        {
            return new TestServer(new WebHostBuilder()
                                  .ConfigureServices(
                                      collection => collection.AddSingleton<IProxyHandler, ProxyHandler>())
                                  .UseStartup<ProxyStartup>()
                                  .UseSetting(Constants.ProxyHandlerRedirectPath, $"http://localhost:{port}"));
        }
    }
}
