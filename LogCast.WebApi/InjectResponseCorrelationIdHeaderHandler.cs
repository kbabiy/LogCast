using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace LogCast.WebApi
{
    /// <summary>
    /// Injects correlationId from current <see cref="LogCastContext"/> into response headers
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class InjectResponseCorrelationIdHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            IEnumerable<string> _;
            var hasCorrelationHeader = CorrelationIdHeader.Accepted
                .Any(name => response.Headers.TryGetValues(name, out _));

            if (!hasCorrelationHeader)
            {
                var correlationId = LogCastContext.Current?.CorrelationId;
                if (correlationId != null)
                    response.Headers.Add(CorrelationIdHeader.Preferred, correlationId);
            }

            return response;
        }
    }
}