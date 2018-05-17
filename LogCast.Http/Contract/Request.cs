using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogCast.Http.Contract
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class Request
    {
        [JsonProperty("uri", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Uri;

        [JsonProperty("http_method", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Method;

        [JsonProperty("headers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string[]> Headers;

        [JsonProperty("claims", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string[]> Claims;

        [JsonProperty("route", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RouteInfo Route;

        [JsonProperty("body", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Body;
    }

    public class RouteInfo
    {
        [JsonProperty("template", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Template;

        [JsonProperty("controller", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Controller;

        [JsonProperty("action", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Action;

        [JsonProperty("values", DefaultValueHandling = DefaultValueHandling.Include)]
        public JObject SerializedValues;
    }
}