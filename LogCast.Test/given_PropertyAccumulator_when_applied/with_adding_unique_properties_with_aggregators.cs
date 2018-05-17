using System.Linq;
using LogCast.Data;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_PropertyAccumulator_when_applied
{
    public class with_adding_unique_properties_with_aggregators : Context
    {
        private ComplexValue _complexValue;

        public override void Arrange()
        {
            base.Arrange();
            _complexValue = new ComplexValue();
            Accumulator.AddProperties(new LogProperty[]
            {
                new LogProperty<int, int>("key1", 34, x => x.Sum() + 1),
                new LogProperty<string, string>("key2", "sample", x => string.Join(":", x) + "fake"),
                new LogProperty<ComplexValue, ComplexValue[]>("key3", _complexValue, x => x.ToArray())
            });
        }

        [Test]
        public void then_their_aggregators_are_ignored()
        {
            Document.GetProperty<int>("key1").Should().Be(34);
            Document.GetProperty<string>("key2").Should().Be("sample");
            Document.GetProperty<ComplexValue>("key3").Should().BeSameAs(_complexValue);
        }
    }
}