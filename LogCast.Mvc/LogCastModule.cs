using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using LogCast.Http;
using LogCast.Http.Contract;
using JetBrains.Annotations;

namespace LogCast.Mvc
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class LogCastModule : HttpInspector, IHttpModule
    {
        private static readonly ILogger Logger = LogManager.GetLogger();

        public LogCastModule() 
            : this(LoggingOptions.Default())
        {
        }

        public LogCastModule(LoggingOptions options) 
            : base(options)
        {
        }

        public virtual void Init(HttpApplication application)
        {
            application.BeginRequest += OnBeginRequest;
            application.EndRequest += OnEndRequest;
        }

        public virtual bool SkipRequest(HttpRequest request)
        {
            return false;
        }

        public virtual string GetCorrelationId(HttpRequest request)
        {
            var headerValue = CorrelationIdHeader.Accepted.Select(k => request.Headers[k]).FirstOrDefault(h => h != null);
            return headerValue ?? Guid.NewGuid().ToString("N");
        }

        public virtual string GetOperationName(HttpRequest request)
        {
            return "default";
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;
            var request = app.Request;

            if (SkipRequest(request))
                return;

            var context = new LogCastContext
            {
                OperationName = GetOperationName(request),
                CorrelationId = GetCorrelationId(request),
                SuppressEmtpyContextMessages = true
            };

            var root = ApplyRequestData(context, request);

            var correlationState = new CorrelationState<Root>(context, root);

            app.Context.Items[CorrelationState<Root>.Key] = correlationState;
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;
            var correlationState = app.Context.Items[CorrelationState<Root>.Key] as CorrelationState<Root>;

            if (correlationState?.Context == null)
                return;

            var response = ((HttpApplication) sender).Response;

            ApplyResponseData(correlationState.Context, correlationState.LogEntry, response);

            correlationState.Context.Dispose();
        }

        private Root ApplyRequestData(LogCastContext context, HttpRequest httpRequest)
        {
            var request = new Request();
            var root = new Root {Request = request};

            try
            {
                if (Options.LogRequestUri)
                {
                    request.Uri = httpRequest.Url.ToString();
                }

                if (Options.LogRequestMethod)
                {
                    request.Method = httpRequest.HttpMethod;
                }

                if (Options.LogRequestHeaders)
                {
                    request.Headers = CollectHeadersSafely(httpRequest.Headers);
                }

                if (Options.LogRequestBody)
                {
                    request.Body = RemoveSensitiveData(httpRequest.Form.ToString());
                }

                context.Properties.Add(Root.FieldName, root);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to log http request data.", ex);
            }
            return root;
        }

        private void ApplyResponseData(LogCastContext context, Root logEntry, HttpResponse httpResponse)
        {
            try
            {
                var response = new Response();
                if (Options.LogResponseStatus)
                {
                    response.Status = httpResponse.StatusCode;
                }

                if (Options.LogResponseHeaders)
                {
                    response.Headers = CollectHeadersSafely(httpResponse.Headers);
                }
                if (Options.LogResponseBody)
                {
                    throw new NotSupportedException();
                }

                if (logEntry == null)
                {
                    logEntry = new Root();
                    context.Properties.Add(Root.FieldName, logEntry);
                }

                logEntry.Response = response;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to log http response data.", ex);
            }
        }

        public virtual void Dispose()
        {
            LogManager.Flush(TimeSpan.FromSeconds(20));
        }
    }
}