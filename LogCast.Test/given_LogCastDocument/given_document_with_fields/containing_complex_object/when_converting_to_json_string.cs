using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields.containing_complex_object
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
        public void then_should_serialize_whole_graph()
        {
            var result = JsonString;
            result.Should().Contain("Property1")
                .And.Contain("Property2")
                .And.Contain("Property3")
                .And.Contain("ChildProperty")
                .And.Contain("rootvalue")
                .And.Contain("23")
                .And.Contain("childvalue");
        }

#pragma warning disable 414
        private class RootObject
        {
            public string Property1;
            public int Property2;
            public ChildObject ChildProperty;
        }

        private class ChildObject
        {
            public string Property3;
        }
    }
}