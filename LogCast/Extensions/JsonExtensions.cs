using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace LogCast.Extensions
{
    public static class JsonExtensions
    {
        public static JObject Flatten(this JObject jObject)
        {
            var result = new JObject();
            foreach (var attribute in Flatten(jObject, string.Empty))
                result[attribute.Key] = attribute.Value;

            return result;
        }

        private static IEnumerable<KeyValuePair<string, JToken>> Flatten(JObject jObject, string prefix)
        {
            return jObject.SelectMany((KeyValuePair<string, JToken> a) => Flatten(Indent(prefix, a)));
        }

        private static IEnumerable<KeyValuePair<string, JToken>> Flatten(JArray array, string prefix)
        {
            return array.SelectMany((e, cnt) => Flatten(Indent(prefix, cnt.ToString(), e)));
        }

        private static IEnumerable<KeyValuePair<string, JToken>> Flatten(KeyValuePair<string, JToken> entry)
        {
            var token = entry.Value;
            if (token == null)
                return Enumerable.Empty<KeyValuePair<string, JToken>>();

            switch (token.Type)
            {
                case JTokenType.Array:
                    return Flatten((JArray) token, entry.Key);
                case JTokenType.Object:
                    return Flatten((JObject) token, entry.Key);
                default:
                    return new[]
                    {
                        new KeyValuePair<string, JToken>(
                            entry.Key,
                            new JValue(((JValue) entry.Value).ToString(CultureInfo.InvariantCulture)))
                    };
            }
        }

        private static KeyValuePair<string, JToken> Indent(string prefix, KeyValuePair<string, JToken> original)
        {
            return Indent(prefix, original.Key, original.Value);
        }

        private static KeyValuePair<string, JToken> Indent(string prefix, string indentation, JToken value)
        {
            var key = prefix.Length == 0
                ? indentation
                : prefix + "." + indentation;
            return new KeyValuePair<string, JToken>(key, value);
        }
    }
}