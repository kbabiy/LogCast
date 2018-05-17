using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields.containing_tab
{
    public class when_converting_to_json_string : Context
    {
        protected override void SetFields(LogCastDocument document)
        {
            document.AddProperty("tab", "\tvalue");
        }

        [Test]
        public void then_slash_symbol_translated_correctly()
        {
            JsonString.Should().Be(AddRootField("{\"tab\":\"\\tvalue\"}"));
        }
    }
}