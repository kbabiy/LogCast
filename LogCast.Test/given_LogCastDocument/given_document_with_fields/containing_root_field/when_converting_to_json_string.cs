using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields.containing_root_field
{
    public class when_converting_to_json_string : Context
    {
        protected override void SetFields(LogCastDocument document)
        {
            document.AddProperty("@root", "root value");
        }

        [Test]
        public void then_root_field_is_on_first_level()
        {
            JsonString.Should().Be("{\"@root\":\"root value\"}");
        }
    }
}