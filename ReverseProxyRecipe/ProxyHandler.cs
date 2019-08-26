using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using ProxyKit;

namespace ReverseProxyRecipe
{
    public class ProxyHandler : IProxyHandler
    {
        private readonly IConfiguration _configuration;

        public ProxyHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HttpResponseMessage> HandleProxyRequest(HttpContext context)
        {
            string redirectPath = _configuration.GetValue(Constants.ProxyHandlerRedirectPath, string.Empty);

            if(string.IsNullOrWhiteSpace(redirectPath))
                throw new InvalidOperationException($"{Constants.ProxyHandlerRedirectPath} is not configured");

            HttpResponseMessage response = await context
                                 .ForwardTo(redirectPath)
                                 .AddXForwardedHeaders()
                                 .ApplyCorrelationId()
                                 .Send();

            if (!response.IsSuccessStatusCode)
            {
                //log error
            }

            return response;
        }
    }
}