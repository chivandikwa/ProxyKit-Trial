using System.Collections.Generic;

namespace ReverseProxyRecipe.Tests
{
    public class ProxyResponse
    {
        public object Content { get; set; }
        public Dictionary<string, string> RequestHeaders { get; set; }
    }
}