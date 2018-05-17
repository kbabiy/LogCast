using System.Collections.Generic;
using LogCast.Data;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_PropertyAccumulator_when_applied
{
    public class with_adding_duplicate_properties : Context
    {
        private ComplexValue _complexValue1;
        private DerivedComplexValue _complexValue2;

        public override void Arrange()
        {
            base.Arrange();

            _complexValue1 = new ComplexValue();
            _complexValue2 = new DerivedComplexValue { DerivedId = 2 };
            Accumulator.AddProperties(new LogProperty[]
            {
                new LogProperty<int>("key1", 34),
                new LogProperty<string>("key2", "sample"),
                new LogProperty<ComplexValue>("key3", _complexValue1),
                new LogProperty<int>("key1", 11),
                new LogProperty<int>("key1", 0),
                new LogProperty<int>("key1", 99),
                new LogProperty<string>("key1", "loser"),
                new LogProperty<ComplexValue>("key3", null),
                new LogProperty<DerivedComplexValue>("key3", _complexValue2)
            });
        }

        [Test]
        public void then_ints_are_joined_as_expected()
        {
            Document.GetProperty<IList<int>>("key1").Should().BeEquivalentTo(new[] { 34, 11, 0, 99 });
        }

        [Test]
        public void then_standalone_property_is_left_standalone()
        {
            Document.GetProperty<string>("key2").Should().Be("sample");
        }

        [Test]
        public void then_complex_values_are_joined_as_expected()
        {
            Document.GetProperty<IList<ComplexValue>>("key3").Should().BeEquivalentTo(_complexValue1, null);
        }
        
        private class DerivedComplexValue : ComplexValue
        {
#pragma warning disable 414
            public int DerivedId;
#pragma warning restore 414
        }
    }
}