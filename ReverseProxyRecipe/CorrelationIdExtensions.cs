using System;

using ProxyKit;

namespace ReverseProxyRecipe
{
    public static class CorrelationIdExtensions
    {
        public const string XCorrelationId = "x-correlation-id";

        public static ForwardContext ApplyCorrelationId(this ForwardContext forwardContext)
        {
            if (!forwardContext.UpstreamRequest.Headers.Contains(XCorrelationId))
            {
                forwardContext.UpstreamRequest.Headers.Add(XCorrelationId, Guid.NewGuid().ToString());
            }
            return forwardContext;
        }
    }
}