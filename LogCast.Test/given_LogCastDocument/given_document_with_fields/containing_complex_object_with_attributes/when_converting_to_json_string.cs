using FluentAssertions;
using LogCast.Engine;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields.containing_complex_object_with_attributes
{
    public class when_converting_to_json_string : Context
    {
        protected override void SetFields(LogCastDocument document)
        {
            document.AddProperty("complex_object", new RootObject
            {
                Property1 = "rootvalue",
                Property2 = 23,
                ChildProperty = new ChildObject
                {
                    Property3 = "childvalue"
                }
            });
        }

        [Test]
        public void then_should_use_name_from_attribute()
        {
            var result = JsonString;
            result.Should().Contain("simple_property")
                .And.Contain("Property2")
                .And.Contain("child1.name.prop")
                .And.Contain("child1.inner.prop");
        }

#pragma warning disable 414

        private class RootObject
        {
            [JsonProperty(PropertyName = "simple_property")]
            public string Property1;

            public int Property2;

            [JsonProperty(PropertyName = "child1.name.prop")]
            public ChildObject ChildProperty;
        }

        private class ChildObject
        {
            [JsonProperty(PropertyName = "child1.inner.prop")]
            public string Property3;
        }
    }
}