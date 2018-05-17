using System;
using System.IO;
using System.Linq;
using LogCast.Http;
using LogCast.Http.Contract;
using Microsoft.Owin;

namespace LogCast.Owin
{
    public class OwinContextLogMap : IOwinContextLogMap
    {
        private readonly MemoryStream _requestBody = new MemoryStream();
        private readonly MemoryStream _responseBody = new MemoryStream();

        private readonly IOwinContext _context;

        private static readonly string[] SensitiveUrls = { "/oauth/token", "/user/reset-password" };

        public OwinContextLogMap(IOwinContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            WrapBodyStreamsWithContentStream(context, _requestBody, _responseBody);
        }

        private static void WrapBodyStreamsWithContentStream(
            IOwinContext context,
            MemoryStream requestBody,
            MemoryStream responseBody)
        {
            if (context.Request.Body != null)
            {
                context.Request.Body = new InterceptionStream(context.Request.Body, requestBody);
            }

            if (context.Response.Body != null)
            {
                context.Response.Body = new InterceptionStream(context.Response.Body, responseBody);
            }
        }

        public void AfterNextHandler(LogCastContext logContext)
        {
            var root = new Root
            {
                Request = ExtractRequestData(_context, _requestBody),
                Response = ExtractResponseData(_context, _responseBody)
            };

            logContext.Properties.Add(Root.FieldName, root);
        }

        private Response ExtractResponseData(IOwinContext owinContext, MemoryStream responseBodyStream)
        {
            var data = new Response
            {
                Headers = HttpInspector.CollectHeadersSafely(owinContext.Request.Headers.ToDictionary(
                    kvp => kvp.Key.ToLowerInvariant(),
                    kvp => kvp.Value.ToArray())),
                Body = responseBodyStream.ReadAsString(),
                Status = _context.Response.StatusCode
            };
            return data;
        }

        private static Request ExtractRequestData(IOwinContext owinContext, MemoryStream requestBodyStream)
        {
            var data = new Request
            {
                Headers = HttpInspector.CollectHeadersSafely(owinContext.Request.Headers.ToDictionary(
                    kvp => kvp.Key.ToLowerInvariant(),
                    kvp => kvp.Value.ToArray())),
                Uri = owinContext.Request.Uri.ToString(),
                Method = owinContext.Request.Method,
                Body = !SensitiveUrls.Contains(owinContext.Request.Uri.AbsolutePath) ? requestBodyStream.ReadAsString() : HttpInspector.RemoveSensitiveData(requestBodyStream.ReadAsString())
            };
            return data;
        }
   }
}
