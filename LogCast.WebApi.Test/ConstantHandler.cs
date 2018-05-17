using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace LogCast.WebApi.Test
{
    public class ConstantHandler<T> : DelegatingHandler
    {
        private readonly T _constant;

        public ConstantHandler(T constant)
        {
            _constant = constant;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage
            {
                Content = new ObjectContent(typeof(T), _constant, new JsonMediaTypeFormatter())
            };

            return Task.FromResult(response);
        }
    }
}