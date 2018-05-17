using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields
{
    public class when_setting_default_value : Context
    {
        protected override void SetFields(LogCastDocument document)
        {
            document.AddProperty("string", (string)null, true);
            document.AddProperty("someKey", "someValue");
            document.AddProperty("int" , 0, true);
        }

        [Test]
        public void then_null_returned_for_reference_type()
        {
            Document.GetProperty<string>("string").Should().BeNull();
        }

        [Test]
        public void then_0_returned_for_int()
        {
            Document.GetProperty<int>("int").Should().Be(0);
        }

        [Test]
        public void then_document_doesn_have_default_fields()
        {
            Document.ToJson().Should().Be(AddRootField(@"{""someKey"":""someValue""}"));
        }
    }
}