using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace LogCast.Wcf.Data
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class Response
    {
        [JsonProperty("properties", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string> Properties;

        [JsonProperty("http_headers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string[]> HttpHeaders;

        [JsonProperty("body", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Body;
    }
}
