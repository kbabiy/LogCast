using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using LogCast.Http;
using JetBrains.Annotations;
using LoggingOptions = LogCast.Wcf.Configuration.LoggingOptions;

namespace LogCast.Wcf
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public abstract class LoggingInspectorBase
    {
        public LoggingOptions Options { get; private set; }

        protected LoggingInspectorBase(LoggingOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // From this point we ignore any changes to LoggingOptions
            Options = options.Clone();
        }

        protected Dictionary<string, string> CollectProperties(MessageProperties properties)
        {
            if (properties == null || properties.Count == 0)
                return null;

            // For HttpRequest/ResponseMessageProperty and RemoteEndpointMessageProperty properties we have dedicated fields
            Dictionary<string, string> target = new Dictionary<string, string>();
            foreach (var property in properties.Where(x => x.Key != HttpRequestMessageProperty.Name 
                && x.Key != HttpResponseMessageProperty.Name && x.Key != RemoteEndpointMessageProperty.Name))
            {
                var value = property.Value.ToString();
                if (value == property.Value.GetType().ToString())
                    continue;

                target[property.Key.ToLowerInvariant()] = value;
            }

            return target;
        }

        protected Dictionary<string, string[]> CollectHttpRequestHeaders(MessageProperties properties)
        {
            if (properties == null || properties.Count == 0)
                return null;

            var httpRequest = GetProperty<HttpRequestMessageProperty>(properties, HttpRequestMessageProperty.Name);
            return httpRequest == null ? null : HttpInspector.CollectHeadersSafely(httpRequest.Headers);
        }

        protected Dictionary<string, string[]> CollectHttpResponseHeaders(MessageProperties properties)
        {
            if (properties == null || properties.Count == 0)
                return null;

            var httpResponse = GetProperty<HttpResponseMessageProperty>(properties, HttpResponseMessageProperty.Name);
            return httpResponse == null ? null : HttpInspector.CollectHeadersSafely(httpResponse.Headers);
        }

        protected string GetCallerAddress(MessageProperties properties)
        {
            if (properties == null || properties.Count == 0)
                return null;

            // For one-way operations, when hosted under IIS, it may return empty string (because request has finished by this time)
            var endpoint = GetProperty<RemoteEndpointMessageProperty>(properties, RemoteEndpointMessageProperty.Name);
            return string.IsNullOrEmpty(endpoint?.Address) ? null : endpoint.Address;
        }

        private static T GetProperty<T>(MessageProperties properties, string propertyName) where T : class
        {
            if (properties.TryGetValue(propertyName, out var value))
                return value as T;

            return null;
        }
    }
}