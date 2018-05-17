using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using LogCast.Extensions;
using LogCast.Http;
using LogCast.Http.Contract;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace LogCast.WebApi
{
    /// <summary>
    /// Adds HTTP request/reponse message information to the passed <see cref="LogCastContext"/> according to the 
    /// provided <see cref="LoggingOptions"/>
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class HttpMessageInspector : HttpInspector, IHttpMessageInspector
    {
        public virtual bool UseGetRequestAsync => Options.LogRequestBody;
        public virtual bool UseGetResponseAsync => Options.LogResponseBody;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpMessageInspector"/> class with default logging options
        /// (all request and response fields except body are added to the current <see cref="LogCastContext"/>)
        /// </summary>
        public HttpMessageInspector() : base(LoggingOptions.Default())
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpMessageInspector"/> class with the provided logging options
        /// </summary>
        public HttpMessageInspector(LoggingOptions options) : base(options)
        {
        }

        public virtual Request GetRequest(LogCastContext target, HttpRequestMessage requestMessage)
        {
            if (!Options.IsRequestLogged)
                return null;

            return PrepareRequestData(requestMessage);
        }

        public virtual Response GetResponse(LogCastContext target, HttpResponseMessage responseMessage)
        {
            if (!Options.IsResponseLogged)
                return null;

            return PrepareResponseData(responseMessage);
        }

        public virtual async Task<Request> GetRequestAsync(LogCastContext target, HttpRequestMessage requestMessage)
        {
            if (!Options.IsRequestLogged)
                return await Task.FromResult<Request>(null);

            Request request = PrepareRequestData(requestMessage);
            if (Options.LogRequestBody && requestMessage.Content != null)
            {
                if (IsMediaContent(requestMessage.Content))
                {
                    request.Body = Property.Values.MediaDataPlaceholder;
                }
                else
                {
                    request.Body = await ReadContentSafelyAsync(requestMessage.Content);
                }
            }
            return request;
        }

        public virtual async Task<Response> GetResponseAsync(LogCastContext target, HttpResponseMessage responseMessage)
        {
            if (!Options.IsResponseLogged)
                return await Task.FromResult<Response>(null);

            var response = PrepareResponseData(responseMessage);
            if (Options.LogResponseBody && responseMessage.Content != null)
            {
                if (IsMediaContent(responseMessage.Content))
                {
                    response.Body = Property.Values.MediaDataPlaceholder;
                }
                else
                {
                    response.Body = await ReadContentSafelyAsync(responseMessage.Content);
                }
            }
            return response;
        }

        private Request PrepareRequestData(HttpRequestMessage message)
        {
            var request = new Request();
            if (Options.LogRequestUri)
            {
                request.Uri = GetRequestUri(message).AbsoluteUri;
            }
            if (Options.LogRequestMethod)
            {
                request.Method = message.Method.Method;
            }
            if (Options.LogRequestHeaders)
            {
                request.Headers = CollectHeadersSafely(message.Headers, message.Content?.Headers);
            }
            if (Options.LogRequestClaims)
            {
                request.Claims = CollectClaims(message);
            }
            if (Options.LogRequestRouteInfo)
            {
                request.Route = CollectRouteData(message);
            }

            return request;
        }

        private Response PrepareResponseData(HttpResponseMessage message)
        {
            var response = new Response();
            if (Options.LogResponseStatus)
            {
                response.Status = (int) message.StatusCode;
            }
            if (Options.LogResponseHeaders)
            {
                response.Headers = CollectHeadersSafely(message.Headers, message.Content?.Headers);
            }

            return response;
        }

        private static Dictionary<string, string[]> CollectClaims(HttpRequestMessage request)
        {
            if (!(request.GetRequestContext()?.Principal is ClaimsPrincipal claimsPrincipal))
                return null;

            Dictionary<string, string[]> result = null;

            foreach (var claimGroup in claimsPrincipal.Claims.GroupBy(x => x.Type))
            {
                var key = claimGroup.Key.ToLowerInvariant();

                if (IsSensitive(key))
                    continue;

                if (result == null)
                    result = new Dictionary<string, string[]>();

                result[key] = claimGroup.Select(x => x.Value).ToArray();
            }

            return result;
        }

        private static readonly string[] SignatureSigns = {"hmac", "rsa", "sha"};

        private static bool IsSensitive(string lowerCasedKey)
        {
            const StringComparison comparisonType = StringComparison.Ordinal;

            if (lowerCasedKey.Equals("swt", comparisonType))
                return true;

            if (lowerCasedKey.Equals("userid", comparisonType))
                return true;

            if (SignatureSigns.Any(x => lowerCasedKey.IndexOf(x, comparisonType) >= 0))
                return true;

            return false;
        }

        private RouteInfo CollectRouteData(HttpRequestMessage request)
        {
            var info = new RouteInfo();

            var routeData = GetRouteData(request);
            if (routeData == null)
                return info;

            var route = routeData.Route;
            if (route == null)
                return info;

            info.Template = route.RouteTemplate;
            var routeValues = routeData.Values;
            if(routeValues != null && routeValues.Any())
                info.SerializedValues = JObject.FromObject(routeValues).Flatten();

            object actionDataToken = null;
            route.DataTokens?.TryGetValue("actions", out actionDataToken);
            HttpActionDescriptor actionDescriptor = (actionDataToken as HttpActionDescriptor[])?.FirstOrDefault();

            if (actionDescriptor != null)
            {
                info.Controller = actionDescriptor.ControllerDescriptor?.ControllerName;
                info.Action = actionDescriptor.ActionName;
            }
            else
            {
                info.Controller = GetRouteValue("controller", routeData.Values, route.Defaults);
                info.Action = GetRouteValue("action", routeData.Values, route.Defaults);
            }
            return info;
        }

        private static string GetRouteValue(
            string key,
            IDictionary<string, object> values,
            IDictionary<string, object> defaults)
        {
            object value = null;
            if (values?.TryGetValue(key, out value) != true)
                defaults?.TryGetValue(key, out value);

            string result = value?.ToString();

            return result;
        }

        internal static IHttpRouteData GetRouteData(HttpRequestMessage request)
        {
            //conventional routing
            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
                return null;

            var subroute = routeData.GetSubRoutes()?.FirstOrDefault();

            //try to resolve for attribute routing
            if (subroute != null)
                routeData = subroute;

            return routeData;
        }

        private Uri GetRequestUri(HttpRequestMessage request)
        {
            request.Headers.TryGetValues(ForwardedHeader, out var values);
            if (values?.Contains("https") == true)
                return EnsureHttps(request.RequestUri);

            return request.RequestUri;
        }

        private bool IsMediaContent(HttpContent content)
        {
            return IsMediaContent(content?.Headers.ContentType?.MediaType);
        }
    }
}