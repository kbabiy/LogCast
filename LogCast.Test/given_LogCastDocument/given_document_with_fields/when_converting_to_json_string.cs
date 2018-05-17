using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields
{
    public class when_converting_to_json_string : Context
    {
        protected override void SetFields(LogCastDocument document)
        {
            document.AddProperty("key1", "value1");
            document.AddProperty("key2", "value2");
        }

        [Test]
        public void then_correct_json_generated()
        {
            JsonString.Should().Be(AddRootField("{\"key1\":\"value1\",\"key2\":\"value2\"}"));
        }
    }
}