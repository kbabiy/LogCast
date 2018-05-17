using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using LogCast.Engine;
using JetBrains.Annotations;

namespace LogCast.Http
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class HttpInspector
    {
        public LoggingOptions Options { get; }

        public const string AuthHeader = "authorization";
        protected const string ForwardedHeader = "X-Forwarded-Proto";

        protected HttpInspector(LoggingOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // From this point we ignore any changes to LoggingOptions
            Options = options.Clone();
        }

        [CanBeNull]
        public static Dictionary<string, string[]> CollectHeadersSafely(
            NameValueCollection headers, bool keysToLower = true)
        {
            if (headers == null || headers.Count == 0)
                return null;

            var result = new Dictionary<string, string[]>();
            foreach (string key in headers.AllKeys)
                ReadHeaderSafely(key, headers.GetValues(key), result, keysToLower);

            return result;
        }

        [CanBeNull]
        public static Dictionary<string, string[]> CollectHeadersSafely(
            HttpHeaders headers, HttpContentHeaders contentHeaders, bool keysToLower = true)
        {
            var result = new Dictionary<string, string[]>();

            if (headers != null)
                foreach (var pair in headers)
                    ReadHeaderSafely(pair.Key, pair.Value, result, keysToLower);

            if (contentHeaders != null)
                foreach (var pair in contentHeaders)
                    ReadHeaderSafely(pair.Key, pair.Value, result, keysToLower);

            if (result.Count == 0)
                result = null;

            return result;
        }

        [CanBeNull]
        public static Dictionary<string, string[]> CollectHeadersSafely(
            IDictionary<string, string[]> headers, bool keysToLower = true)
        {
            var result = new Dictionary<string, string[]>();

            if (headers != null)
                foreach (var pair in headers)
                    ReadHeaderSafely(pair.Key, pair.Value, result, keysToLower);

            if (result.Count == 0)
                result = null;

            return result;
        }

        public static async Task<string> ReadContentSafelyAsync(HttpContent content)
        {
            if (content == null)
                return await Task.FromResult<string>(null);

            try
            {
                var stream = await content.ReadAsStreamAsync();
                var result = await stream.ReadSafelyAsync();
                return RemoveSensitiveData(result);
            }
            catch (Exception e)
            {
                return "Failed to read content: " + e;
            }
        }

        private static readonly string[] SensitiveInformationKeywords = { "password", "newPassword", "oldPassword", "access_token", "refresh_token" };
        public static string RemoveSensitiveData(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return null;

            if (SensitiveInformationKeywords.All(x => body.IndexOf(x, StringComparison.OrdinalIgnoreCase) == -1))
                return body;

            try
            {
                var jsonBody = JObject.Parse(body);
                if (jsonBody == null)
                    return Property.Values.Removed;

                var keys = ((IDictionary<string, JToken>) jsonBody)
                    .Where(x => SensitiveInformationKeywords.Contains(x.Key, StringComparer.OrdinalIgnoreCase))
                    .Select(x => x.Key);

                foreach (var keyValuePair in keys)
                {
                    jsonBody[keyValuePair] = Property.Values.Removed;
                }

                return LogCastDocument.ToJson(jsonBody);
            }
            catch (Exception)
            {
                return Property.Values.Removed;
            }
        }

        private static void ReadHeaderSafely(string key, IEnumerable<string> values, 
            Dictionary<string, string[]> target, bool keyToLower)
        {
            if (keyToLower)
                key = key.ToLowerInvariant();

            var val = !string.Equals(key, AuthHeader, StringComparison.OrdinalIgnoreCase)
                ? values
                : values.Select(h =>
                    {
                        var parts = h.Split(' ');
                        return parts.Length == 2
                            ? $"{parts[0]} {Property.Values.Removed}"
                            : Property.Values.Removed;
                    }
                );

            target[key] =val.ToArray();
        }

        protected bool IsMediaContent(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return false;

            return contentType.StartsWith("image") || contentType.StartsWith("audio") || contentType.StartsWith("video");
        }

        protected Uri EnsureHttps(Uri requestUrl)
        {
            var builder = new UriBuilder(requestUrl) {Scheme = "https"};
            if (builder.Port == 80)
            {
                builder.Port = 443;
            }

            return builder.Uri;
        }
    }
}