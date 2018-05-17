using System;
using System.Collections.Generic;
using System.Linq;
using LogCast.Engine;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace LogCast.Data
{
    /// <summary>
    /// Represents attribute added to the resulting log message
    /// </summary>
    public abstract class LogProperty
    {
        /// <summary>
        /// Key to group by when aggregating properties
        /// </summary>
        public virtual string Key => Name;
        protected string Name { get; }

        /// <summary>
        /// Stores property value to the passed <see cref="LogCastDocument"/>
        /// </summary>
        protected internal abstract void Apply(LogCastDocument document);

        /// <summary>
        /// Aggregates values of the passed properties and stores the result to the passed <see cref="LogCastDocument"/>
        /// This method is called only when there's more than one property with the same name
        /// </summary>
        protected internal abstract void Apply(LogCastDocument document, IEnumerable<LogProperty> properties);

        protected LogProperty([NotNull]string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }
    }

    /// <summary>
    /// <see cref="LogProperty"/> that with typed value
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public abstract class LogPropertyWithValue<T> : LogProperty
    {
        public T Value { get; }

        /// <summary>
        /// When 'true' the property will not add the attribute to log message if it has default values
        /// </summary>
        public bool SuppressDefaults { get; set; }

        protected LogPropertyWithValue([NotNull]string name, T value) : base(name)
        {
            Value = value;
        }

        protected internal sealed override void Apply(LogCastDocument document)
        {
            Apply(document, Value);
        }

        protected virtual void Apply<TDestination>(LogCastDocument document, TDestination value)
        {
            var existingValue = document.GetProperty<object>(Key);

            if (existingValue is Dictionary<string, object> childDictionary)
                childDictionary[Property.DefaultChildName] = value;
            else
                document.AddProperty(Name, value, SuppressDefaults);
        }
    }

    /// <summary>
    /// <see cref="LogProperty"/> with custom aggregation logic
    /// </summary>
    public class LogProperty<TValue, TAggregatedValue> : LogPropertyWithValue<TValue>
    {
        public Func<IEnumerable<TValue>, TAggregatedValue> Aggregator { get; }

        /// <param name="name">A name of the property as it should be displayed in logs</param>
        /// <param name="value">Can be either primitive type or an object
        /// The object will be serialized with Newtonsoft.Json serializer so you can apply <see cref="JsonPropertyAttribute"/> 
        /// to the object's properties to customize the serialization logic.
        /// Important: when changing type of the value always test your changes</param>
        /// <param name="aggregator">A LINQ expression to apply when there is more than one property with the same name</param>
        public LogProperty([NotNull]string name, TValue value, [NotNull] Func<IEnumerable<TValue>, TAggregatedValue> aggregator)
            : base(name, value)
        {
            Aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
        }
        
        private TAggregatedValue Aggregate(IEnumerable<LogProperty> properties)
        {
            return Aggregator(properties.OfType<LogPropertyWithValue<TValue>>().Select(x => x.Value));
        }

        protected internal sealed override void Apply(LogCastDocument document, IEnumerable<LogProperty> properties)
        {
            var value = Aggregate(properties);
            Apply(document, value);
        }
    }

    /// <summary>
    /// <see cref="LogProperty"/> with default aggregation logic
    /// </summary>
    public class LogProperty<T> : LogProperty<T, T[]>
    {
        /// <param name="name">A name of the property as it should be displayed in logs</param>
        /// <param name="value">Can be either primitive type or an object
        /// The object will be serialized with Newtonsoft.Json serializer so you can apply <see cref="JsonPropertyAttribute"/> 
        /// to the object's properties to customize the serialization logic.
        /// Important: when changing type of the value always test your changes</param>
        public LogProperty([NotNull]string name, T value)
            : base(name, value, enumerable => enumerable.ToArray())
        { }
    }
}