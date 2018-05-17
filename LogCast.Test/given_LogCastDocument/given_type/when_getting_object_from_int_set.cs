using BddStyle.NUnit;
using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_type
{
    public class when_getting_object_from_int_set : ContextBase
    {
        private LogCastDocument _cut;
        public override void Arrange()
        {
            _cut = new LogCastDocument();
        }

        public override void Act()
        {
            _cut.AddProperty("key", 10);
        }

        [Test]
        public void then_object_is_what_was_set()
        {
            _cut.GetProperty<object>("key").Should().NotBeNull().And.Be(10);
        }
    }
}