using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields.containing_carriage_return
{
    public class when_converting_to_json_string : Context
    {
        protected override void SetFields(LogCastDocument document)
        {
            document.AddProperty("carriage_return" , "\rvalue");
        }

        [Test]
        public void then_new_line_symbol_translated_correctly()
        {
            JsonString.Should().Be(AddRootField("{\"carriage_return\":\"\\rvalue\"}"));
        }
    }
}