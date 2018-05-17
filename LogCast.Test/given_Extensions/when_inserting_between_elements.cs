using LogCast.Extensions;
using BddStyle.NUnit;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_Extensions
{
    public class when_inserting_between_elements : ContextBase
    {
        private const int Extra = 42;
        private readonly int[] _initial = { 1, 2, 3, 4 };
        private int[] _result;

        public override void Act()
        {
            _result = _initial.InsertBetween(_initial.Length, Extra);
        }

        [Test]
        public void then_result_is_as_expected()
        {
            _result.Should().BeEquivalentTo(new[] { 1, Extra, 2, Extra, 3, Extra, 4 });
        }
    }
}