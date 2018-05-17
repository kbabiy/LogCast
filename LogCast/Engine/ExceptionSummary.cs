using System.Collections.Generic;
using Newtonsoft.Json;

namespace LogCast.Engine
{
    public class ExceptionSummary
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Include)]
        public HashSet<string> Types;

        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Include)]
        public string[] Values;
    }
}