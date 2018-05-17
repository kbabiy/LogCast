using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields.containing_quote
{
    public class when_converting_to_json_string : Context
    {
        protected override void SetFields(LogCastDocument document)
        {
            document.AddProperty("quote", "\"value");
        }

        [Test]
        public void then_quote_symbol_translated_correctly()
        {
            JsonString.Should().Be(AddRootField("{\"quote\":\"\\\"value\"}"));
        }
    }
}