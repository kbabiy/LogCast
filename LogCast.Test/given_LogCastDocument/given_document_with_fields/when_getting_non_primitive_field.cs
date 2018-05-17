using System.Collections.Generic;
using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields
{
    public class when_getting_non_primitive_field : Context
    {
        private Dictionary<string, int> _value;
         
        protected override void SetFields(LogCastDocument document)
        {
            _value = new Dictionary<string, int> {{"key1", 23}, { "key2", 24 } };
            document.AddProperty("key" , _value);
        }

        [Test]
        public void then_field_value_returned()
        {
            Document.GetProperty<Dictionary<string, int>>("key").Should().BeSameAs(_value);
        }
    }
}