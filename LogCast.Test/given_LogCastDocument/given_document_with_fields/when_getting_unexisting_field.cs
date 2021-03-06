﻿using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_document_with_fields
{
    public class when_getting_unexisting_field : Context
    {
        protected override void SetFields(LogCastDocument document)
        {
            document.AddProperty("key", "value");
        }

        [Test]
        public void then_field_value_returned()
        {
            Document.GetProperty<object>("unexisting_key").Should().BeNull();
        }
    }
}