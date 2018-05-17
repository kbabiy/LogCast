using System.Collections.Generic;
using System.Linq;
using LogCast.Data;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_PropertyAccumulator_when_applied
{
    public class with_adding_complex_properties : Context
    {
        public override void Arrange()
        {
            base.Arrange();
            Accumulator.AddProperties(new LogProperty[]
            {
                new ComplexLogProperty<int>("parent", "single", 10),
                new ComplexLogProperty<int>("parent", "duplicate", 15),
                new ComplexLogProperty<int>("parent", "duplicate", 3),
                new ComplexLogProperty<int, int>("parent", "aggregate", 30, ints => ints.Sum()),
                new ComplexLogProperty<int, string>("parent", "aggregate", 4, ints => "something"),
                new ComplexLogProperty<int>("parent", "aggregate", 6),

                new LogProperty<int>("standalone", 42),

                new LogProperty<int, string>("existing", 39, ints => "agg"),
                new ComplexLogProperty<int>("existing", "aggregate", 6),
                new LogProperty<int>("existing", 1024),

                new ComplexLogProperty<int>("existing2", "aggregate", 6),
                new LogProperty<int, string>("existing2", 39, ints => "agg")
            });
        }

        [Test]
        public void then_single_key_is_added_to_child_container()
        {
            Document.GetProperty<int>("parent", "single").Should().Be(10);
        }

        [Test]
        public void then_duplicated_value_is_aggregated_as_array()
        {
            Document.GetProperty<object>("parent", "duplicate").Should().BeEquivalentTo(new[] { 15, 3 });
        }

        [Test]
        public void then_custom_aggregation_logic_from_first_property_is_applied()
        {
            Document.GetProperty<int>("parent", "aggregate").Should().Be(40);
        }

        [Test]
        public void then_standalone_simple_property_remains()
        {
            Document.GetProperty<int>("standalone").Should().Be(42);
        }

        [Test]
        public void then_existing_property_is_applied_to_the_document()
        {
            Document.GetProperty<object>("existing").Should().BeEquivalentTo(
                new Dictionary<string, object>
                {
                    {"value", "agg"},
                    {"aggregate", 6}
                });
        }

        [Test]
        public void then_existing_trailing_simple_property_is_aggregated_with_existing()
        {
            Document.GetProperty<object>("existing2").Should().BeEquivalentTo(
                new Dictionary<string, object>
                {
                    {"value", 39},
                    {"aggregate", 6}
                });
        }
    }
}