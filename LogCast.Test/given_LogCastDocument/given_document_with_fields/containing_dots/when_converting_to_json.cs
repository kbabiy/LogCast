using System.Collections.Generic;
using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields.containing_dots
{
    public class when_converting_to_json : Context
    {
        protected override void SetFields(LogCastDocument document)
        {
            document.AddProperty("key1", "simple.value1");
            document.AddProperty("simple.property", "simple.value2");
            document.AddProperty("dictionary.property", 
                new Dictionary<string, string>
                {
                    { "key", "value"},
                    { "key.with.dots", "value.with.dots"}
                });

        }

        [Test]
        public void then_dots_are_not_escaped_in_attribute_names()
        {
            JsonString.Should().Be(AddRootField("{\"key1\":\"simple.value1\",\"simple.property\":\"simple.value2\",\"dictionary.property\":{\"key\":\"value\",\"key.with.dots\":\"value.with.dots\"}}"));
        }
    }
}