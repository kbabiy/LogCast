using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using LogCast.Engine;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_request.and_route_enabled
{
    public class when_complex_route_values_accepted : Context
    {
        protected override bool SuppressAct => true;
        protected override IDictionary<string, object> RouteValues => new Dictionary<string, object> {{"id", null}};

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static IEnumerable<TestCaseData> Cases
        {
            get
            {
                yield return new TestCaseData(new Dictionary<string, object>
                        {
                            {"key1", "value1"},
                            {"complexKey", new Dictionary<string, object> {{"key2", "value2"}, {"key3", "value3"}}}
                        },
                        @"{""id.key1"":""value1"",""id.complexKey.key2"":""value2"",""id.complexKey.key3"":""value3""}")
                    .SetName("dictionary");

                yield return new TestCaseData(
                        42, Json(new {id = "42"}))
                    .SetName("int");

                yield return new TestCaseData(
                        12.5, Json(new{id = "12.5"}))
                    .SetName("float");

                yield return new TestCaseData(
                        "some string", Json(new { id = "some string" }))
                    .SetName("string");

                yield return new TestCaseData(
                        new object[] {1, 2, "heyho"}, @"{""id.0"":""1"",""id.1"":""2"",""id.2"":""heyho""}")
                    .SetName("array");

                var typeString = new MyType { Type = "string" };
                yield return new TestCaseData(
                        new MyContract { Props = new MyProperties { Controller = typeString, Id = typeString, Size = typeString } },
                        @"{""id.properties.controller.type"":""string"",""id.properties.id.type"":""string"",""id.properties.size.type"":""string""}")
                    .SetName("datacontract");
            }
        }

        private void Act(object innerFieldValue)
        {
            var vals = RequestMessage.GetRequestContext().RouteData.Values;
            vals["id"] = innerFieldValue;
            Act();
        }

        private static string Json(object obj)
        {
            return LogCastDocument.ToJson(obj);
        }

        [TestCaseSource(nameof(Cases))]
        public void then_flattenned_correctly(object input, string expected)
        {
            Act(input);
            var actual = LogCastDocument.ToJson(Request.Route.SerializedValues);

            actual.Should().Be(expected);
        }

        public class MyContract
        {
            [JsonProperty("properties")]
            public MyProperties Props { get; set; }
        }

        public class MyProperties
        {
            [JsonProperty("controller")]
            public MyType Controller { get; set; }

            [JsonProperty("id")]
            public MyType Id { get; set; }

            [JsonProperty("size")]
            public MyType Size { get; set; }
        }

        public class MyType
        {
            [JsonProperty("type")]
            public string Type { get; set; }
        }
    }
}