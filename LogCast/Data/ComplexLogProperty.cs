using System;
using System.Collections.Generic;
using System.Linq;
using LogCast.Engine;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace LogCast.Data
{
    public class ComplexLogProperty<TValue, TAggregatedValue> : LogProperty<TValue, TAggregatedValue>
    {
        public override string Key { get; }
        public string ParentName { get; }

        /// <param name="parentName">A name of the complex object root</param>
        /// <param name="name">A name of the property as it should be displayed in logs</param>
        /// <param name="value">Can be either primitive type or an object
        /// The object will be serialized with Newtonsoft.Json serializer so you can apply <see cref="JsonPropertyAttribute"/> 
        /// to the object properties to customize the serialization logic
        /// Important: when changing type of the value always test your changes</param>
        /// <param name="aggregator">A LINQ expression to apply when there is more than one property with the same name</param>
        public ComplexLogProperty([NotNull]string parentName, [NotNull]string name, TValue value,
            [NotNull] Func<IEnumerable<TValue>, TAggregatedValue> aggregator)
            : base(name, value, aggregator)
        {
            if (parentName == null)
                throw new ArgumentNullException(nameof(parentName));

            if(name.Equals(Property.DefaultChildName))
                throw new ArgumentException($"ComplexLogProperty usage with {nameof(name)}='{name}' is not supported. Use LogProperty instead");

            Key = parentName + "." + name;
            ParentName = parentName;
        }

        protected override void Apply<TDestination>(LogCastDocument document, TDestination value)
        {
            var existingValue = document.GetProperty<object>(ParentName);

            if (existingValue is Dictionary<string, object> childDictionary)
            {
                existingValue = null;
            }
            else
            {
                childDictionary = new Dictionary<string, object>();
                document.AddProperty(ParentName, childDictionary);
            }

            if (existingValue != null)
                childDictionary[Property.DefaultChildName] = existingValue;

            childDictionary[Name] = value;
        }
    }

    public class ComplexLogProperty<T> : ComplexLogProperty<T, T[]>
    {
        /// <param name="parentName">A name of the complex object root</param>
        /// <param name="name">A name of the property as it should be displayed in logs</param>
        /// <param name="value">Can be either primitive type or an object
        /// The object will be serialized with Newtonsoft.Json serializer so you can apply <see cref="JsonPropertyAttribute"/> 
        /// to the object's properties to customize the serialization logic.
        /// Important: when changing type of the value always test your changes</param>
        public ComplexLogProperty([NotNull] string parentName, [NotNull] string name, T value)
            : base(parentName, name, value, enumerable => enumerable.ToArray())
        { }
    }
}