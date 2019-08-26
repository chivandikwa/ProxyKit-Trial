using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ReverseProxyRecipe
{
    public static class HttpClientExtensions
    {
        public static HttpMessageHandler CreatePrimaryHandler()
        {
            var clientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            return clientHandler;
        }

        public static void AddMediaTypeWithQualityHeaderValue(this HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}