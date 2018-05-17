using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;

namespace LogCast.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static string GetHeadersCorrelationId(this HttpRequestMessage request)
        {
            return CorrelationIdHeader.Accepted
                .Select(request.GetHeaderValue)
                .FirstOrDefault(correlationId => !string.IsNullOrEmpty(correlationId));
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static string GetDefaultOperationName(this HttpRequestMessage request)
        {
            return "default operation strategy";
        }

        private static string GetHeaderValue(this HttpRequestMessage request, string headerName)
        {
            return request.Headers.TryGetValues(headerName, out var values)
                ? values.FirstOrDefault()
                : null;
        }
    }
}