using System.Linq;
using LogCast.Data;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_PropertyAccumulator_when_applied
{
    public class with_adding_duplicate_properties_with_aggregators : Context
    {
        private ComplexValue _complexValue1;
        private ComplexValue _complexValue2;

        public override void Arrange()
        {
            base.Arrange();
            _complexValue1 = new ComplexValue { Id = 10, Name = "one" };
            _complexValue2 = new ComplexValue { Id = 20, Name = "two" };
            Accumulator.AddProperties(new LogProperty[]
            {
                new LogProperty<int, int>("key1", 1, x => x.Sum()),
                new LogProperty<string>("key2", "sample"),
                new LogProperty<ComplexValue, string>("key3", _complexValue1, x => string.Concat(x.Select(y => y?.Name ?? "[null]"))),
                new LogProperty<int, int[]>("key1", 2, x => x.ToArray()),
                new LogProperty<int, string>("key1", 3, string.Concat),
                new LogProperty<int, int>("key1", 4, x => x.Max()),
                new LogProperty<ComplexValue>("key3", _complexValue2),
                new LogProperty<ComplexValue, int>("key3", null, x => x.Select(y => y.Id).Max())
            });
        }

        [Test]
        public void then_their_values_are_joined_according_to_first_property_aggregator()
        {
            var property1 = Document.GetProperty<int>("key1");
            property1.Should().Be(1 + 2 + 3 + 4);

            Document.GetProperty<string>("key2").Should().Be("sample");

            var property3 = Document.GetProperty<string>("key3");
            property3.Should().Be(_complexValue1.Name + _complexValue2.Name + "[null]");
        }
    }
}