using System.Net;

namespace ReverseProxyRecipe.Tests
{
    public class ProxyContext
    {
        public HttpStatusCode StatusCode { get; set; }
        public object Content { get; set; }
    }
}