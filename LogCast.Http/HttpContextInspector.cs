using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web;
using LogCast.Engine;
using System.Linq;
using LogCast.Http.Contract;
using LogCast.Inspectors;
using JetBrains.Annotations;

namespace LogCast.Http
{
    /// <summary>
    /// When registered as LogCast inspector, adds information from current <see cref="HttpContext"/> 
    /// to each logging message sent according to the passed <see cref="LoggingOptions"/>
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class HttpContextInspector : HttpInspector, ILogDispatchInspector
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HttpContextInspector"/> class with default logging options
        /// </summary>
        public HttpContextInspector() : base(LoggingOptions.Default())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpContextInspector"/> class with the provided logging options
        /// </summary>
        public HttpContextInspector(LoggingOptions options) : base(options)
        {
        }

        public void BeforeSend(LogCastDocument document, LogMessage sourceMessage)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                Apply(document, context);
            }
        }

        public void BeforeSend(LogCastDocument document, LogCastContext sourceContext)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                Apply(document, context);
            }
        }

        protected virtual void Apply(LogCastDocument target, HttpContext context)
        {
            var root = new Root();

            if (Options.IsRequestLogged)
            {
                var request = context.Request;
                var requestData = new Request();
                if (Options.LogRequestUri)
                {
                    requestData.Uri = GetRequestUri(request).AbsoluteUri;
                }
                if (Options.LogRequestMethod)
                {
                    requestData.Method = request.HttpMethod;
                }
                if (Options.LogRequestHeaders)
                {
                    requestData.Headers = CollectHeadersSafely(request.Headers);
                }
                if (Options.LogRequestBody)
                {
                    requestData.Body = ReadRequestBody(request);
                }

                root.Request = requestData;
            }

            if (Options.IsResponseLogged)
            {
                var response = context.Response;
                var responseData = new Response();
                if (Options.LogResponseStatus)
                {
                    responseData.Status = response.StatusCode;
                }
                if (Options.LogResponseHeaders)
                {
                    responseData.Headers = CollectHeadersSafely(response.Headers);
                }
                if (Options.LogResponseBody)
                {
                    responseData.Body = ReadResponseBody(response);
                }

                root.Response = responseData;
            }

            if (root.HasData)
                target.AddProperty(Root.FieldName, root);
        }

        private string ReadRequestBody(HttpRequest request)
        {
            if (IsMediaContent(request.ContentType))
                return Property.Values.MediaDataPlaceholder;

            try
            {
                // We need try/catch because InputStream property accessor may throw an exception
                return ReadHttpBodyStream(request.InputStream);
            }
            catch (Exception ex)
            {
                return "Failed to read request body. Error: " + ex;
            }
        }

        private string ReadResponseBody(HttpResponse response)
        {
            if (IsMediaContent(response.ContentType))
                return Property.Values.MediaDataPlaceholder;

            try
            {
                // We need try/catch because OutputStream property accessor may throw an exception
                return ReadHttpBodyStream(response.OutputStream);
            }
            catch (Exception ex)
            {
                return "Failed to read response body. Error: " + ex;
            }
        }

        private Uri GetRequestUri(HttpRequest request)
        {
            string[] values = request.Headers.GetValues(ForwardedHeader);
            if (values?.Contains("https") == true)
                return EnsureHttps(request.Url);

            return request.Url;
        }

        private string ReadHttpBodyStream(Stream stream)
        {
            var result = stream.ReadSafely();
            return RemoveSensitiveData(result);
        }
    }
}