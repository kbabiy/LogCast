using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LogCast.Http.Contract;
using JetBrains.Annotations;
using LogCast.Http;

namespace LogCast.WebApi
{
    /// <summary>
    /// Creates a <see cref="LogCastContext"/> for each WebApi request
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class LogCastContextHandler : DelegatingHandler
    {
        private static readonly ILogger Logger = LogManager.GetLogger();

        private readonly Func<HttpRequestMessage, string> _correlationIdProvider;
        private readonly Func<HttpRequestMessage, string> _operationNameProvider;
        public IHttpMessageInspector MessageInspector { get; private set; }

        private static readonly string DefaultOperationStrategy = Http.HttpRequestMessageExtensions.GetDefaultOperationName(null);

        public LogCastContextHandler() : this(new HttpMessageInspector())
        {
        }

        /// <param name="messageInspector">An object that collects request/response data and adds it to the active <see cref="LogCastContext"/>
        /// If null - then request/response data is not collected</param>
        public LogCastContextHandler([CanBeNull] IHttpMessageInspector messageInspector)
            : this(r => r.GetHeadersCorrelationId(), r => r.GetDefaultOperationName(), messageInspector)
        {
        }

        /// <param name="correlationIdProvider">A delegate that returns CorrelationId based on the passed <see cref="HttpRequestMessage"/></param>
        /// <param name="operationNameProvider">A delegate that returns operation name based on the passed <see cref="HttpRequestMessage"/>
        /// If null - then default operation name will be used ('SendAsync')</param>
        /// <param name="messageInspector">An object that collects request/response data and adds it to the active <see cref="LogCastContext"/>
        /// If null - then request/response data is not collected</param>
        public LogCastContextHandler(
            [CanBeNull] Func<HttpRequestMessage, string> correlationIdProvider,
            [CanBeNull] Func<HttpRequestMessage, string> operationNameProvider,
            [CanBeNull] IHttpMessageInspector messageInspector)
        {
            _correlationIdProvider = correlationIdProvider;
            _operationNameProvider = operationNameProvider;
            MessageInspector = messageInspector;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            using (var context = new LogCastContext())
            {
                HttpResponseMessage response;
                try
                {
                    var correlationId = _correlationIdProvider?.Invoke(request);
                    if (correlationId != null)
                        context.CorrelationId = _correlationIdProvider(request);

                    var httpRoot = new Root();
                    if (MessageInspector != null)
                    {
                        httpRoot.Request = MessageInspector.UseGetRequestAsync
                            ? await MessageInspector.GetRequestAsync(context, request)
                            : MessageInspector.GetRequest(context, request);
                    }

                    var operationName = _operationNameProvider?.Invoke(request);
                    if (operationName == DefaultOperationStrategy)
                    {
                        var routeInfo = httpRoot.Request?.Route;
                        operationName = routeInfo != null
                            ? routeInfo.Template
                            : HttpMessageInspector.GetRouteData(request)?.Route?.RouteTemplate;
                    }

                    if (operationName != null)
                        context.OperationName = operationName;

                    response = await base.SendAsync(request, cancellationToken);

                    if (MessageInspector != null)
                    {
                        httpRoot.Response = MessageInspector.UseGetResponseAsync
                            ? await MessageInspector.GetResponseAsync(context, response)
                            : MessageInspector.GetResponse(context, response);
                    }

                    if (httpRoot.HasData)
                        context.Properties.Add(Root.FieldName, httpRoot);
                }
                catch (Exception ex)
                {
                    // This catch is only for exceptions raised in subsequent handlers. It doesn't catch action errors
                    Logger.Error(ex);
                    throw;
                }

                return response;
            }
        }
    }
}