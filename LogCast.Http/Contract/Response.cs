using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace LogCast.Http.Contract
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class Response
    {
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Status;

        [JsonProperty("headers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string[]> Headers;

        [JsonProperty("body", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Body;
    }
}
