using System.Collections.Generic;
using System.Linq;
using LogCast.Data;

namespace LogCast.Engine
{
    internal class PropertyAccumulator
    {
        private readonly Dictionary<string, object> _properties;

        public PropertyAccumulator()
        {
            _properties = new Dictionary<string, object>(LogCastDocument.FieldNamesComparer);
        }

        public void AddProperties(IEnumerable<LogProperty> properties)
        {
            foreach (var property in properties.Where(x => x != null))
            {
                if (!_properties.TryGetValue(property.Key, out var existingProperty))
                {
                    // The most common case: a property presents one time only (so we avoid creating the list until it's necessary)
                    _properties.Add(property.Key, property);
                }
                else
                {
                    if (existingProperty is List<LogProperty> list)
                        list.Add(property);
                    else
                        _properties[property.Key] = new List<LogProperty> {(LogProperty) existingProperty, property};
                }
            }
        }

        public void Apply(LogCastDocument document)
        {
            foreach (var pair in _properties)
            {
                if (pair.Value is LogProperty property)
                {
                    property.Apply(document);
                }
                else
                {
                    var list = (List<LogProperty>)pair.Value;

                    // Aggregator of the first property defines the aggregation logic
                    list[0].Apply(document, list);
                }
            }
        }
    }
}
