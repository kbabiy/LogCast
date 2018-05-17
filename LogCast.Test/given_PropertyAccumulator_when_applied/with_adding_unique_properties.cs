using LogCast.Data;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_PropertyAccumulator_when_applied
{
    public class with_adding_unique_properties : Context
    {
        private ComplexValue _complexValue;

        public override void Arrange()
        {
            base.Arrange();
            _complexValue = new ComplexValue();
            Accumulator.AddProperties(new LogProperty[]
            {
                new LogProperty<int>("key1", 34),
                new LogProperty<string>("key2", "sample"),
                new LogProperty<ComplexValue>("key3", _complexValue),
                // Test default values
                new LogProperty<int>("int1", 0) {SuppressDefaults = true},
                new LogProperty<int>("int2", 0),
                new LogProperty<int?>("nullable1", null) {SuppressDefaults = true},
                new LogProperty<int?>("nullable2", null),
                new LogProperty<int?>("nullable3", 0) {SuppressDefaults = true},
                new LogProperty<string>("string1", null) {SuppressDefaults = true},
                new LogProperty<string>("string2", null),
                new LogProperty<string>("string3", ""){SuppressDefaults = true},
                new LogProperty<ComplexValue>("complex1", null) {SuppressDefaults = true},
                new LogProperty<ComplexValue>("complex2", null)
            });
        }

        [Test]
        public void then_their_values_are_applied_and_type_is_preserved()
        {
            Document.GetProperty<int>("key1").Should().Be(34);
            Document.GetProperty<string>("key2").Should().Be("sample");
            Document.GetProperty<ComplexValue>("key3").Should().BeSameAs(_complexValue);

            // These values are not default so they also must be present
            Document.PropertyExists("nullable3").Should().BeTrue();
            Document.GetProperty<int?>("nullable3").Should().Be(0);
            Document.PropertyExists("string3").Should().BeTrue();
            Document.GetProperty<string>("string3").Should().Be("");
        }

        [Test]
        public void then_non_mandatory_properties_with_defaul_values_are_skipped()
        {
            Document.PropertyExists("int1").Should().BeFalse();
            Document.PropertyExists("nullable1").Should().BeFalse();
            Document.PropertyExists("string1").Should().BeFalse();
            Document.PropertyExists("complex1").Should().BeFalse();
        }

        [Test]
        public void then_mandatory_properties_with_defaul_values_are_preserved()
        {
            Document.PropertyExists("int2").Should().BeTrue();
            Document.GetProperty<int>("int2").Should().Be(0);

            Document.PropertyExists("nullable2").Should().BeTrue();
            Document.GetProperty<int?>("nullable2").Should().Be(null);

            Document.PropertyExists("string2").Should().BeTrue();
            Document.GetProperty<string>("string2").Should().Be(null);

            Document.PropertyExists("complex2").Should().BeTrue();
            Document.GetProperty<ComplexValue>("complex2").Should().Be(null);
        }
    }
}