using System;
using System.Collections.Generic;
using LogCast.Data;

namespace LogCast.Context
{
    /// <summary>
    /// A collection for storing custom properties of LogCast context
    /// </summary>
    public class ContextPropertiesCollection : ContextDataCollection<LogProperty>
    {
        /// <summary>
        /// Adds a new property to the LogCast context. This method is thread-safe
        /// </summary>
        /// <param name="propertyName">A name of the property as it should be displayed in logs</param>
        /// <param name="propertyValue">Can be either primitive type or an object</param>
        /// <returns>Returns 'true' if the property has been added to the collection. May return 
        /// 'False' if the collection entered the read-only mode</returns>
        public bool Add<T>(string propertyName, T propertyValue)
        {
            return Add(new LogProperty<T>(propertyName, propertyValue));
        }

        /// <summary>
        /// Adds a new property with custom aggregation logic to the LogCast context. This method is thread-safe
        /// </summary>
        /// <param name="propertyName">A name of the property as it should be displayed in logs</param>
        /// <param name="propertyValue">Can be either primitive type or an object</param>
        /// <param name="aggregator">A LINQ expression to apply when there is more than one property with the same name</param>
        /// <returns>Returns 'true' if the property has been added to the collection. May return 
        /// 'False' if the collection entered the read-only mode</returns>
        public bool Add<TValue, TAggregatedValue>(string propertyName, TValue propertyValue,
            Func<IEnumerable<TValue>, TAggregatedValue> aggregator)
        {
            return Add(new LogProperty<TValue, TAggregatedValue>(propertyName, propertyValue, aggregator));
        }

        /// <summary>
        /// Adds a new property to the object within LogCast context. This method is thread-safe
        /// </summary>
        /// <param name="containerName">Name of the complex object</param>
        /// <param name="propertyName">A name of the nested property as it should be displayed in logs</param>
        /// <param name="propertyValue">Can be either primitive type or an object</param>
        /// <returns>Returns 'true' if the property has been added to the collection. May return 
        /// 'False' if the collection entered the read-only mode</returns>
        public bool Add<T>(string containerName, string propertyName, T propertyValue)
        {
            return Add(new ComplexLogProperty<T>(containerName, propertyName, propertyValue));
        }

        /// <summary>
        /// Adds a new property with custom aggregation logic to the object within LogCast context. This method is thread-safe
        /// </summary>
        /// <param name="containerName">Name of the complex object</param>
        /// <param name="propertyName">A name of the nested property as it should be displayed in logs</param>
        /// <param name="propertyValue">Can be either primitive type or an object</param>
        /// <param name="aggregator">A LINQ expression to apply when there is more than one property with the same name</param>
        /// <returns>Returns 'true' if the property has been added to the collection. May return 
        /// 'False' if the collection entered the read-only mode</returns>
        public bool Add<TValue, TAggregatedValue>(string containerName, string propertyName, TValue propertyValue,
            Func<IEnumerable<TValue>, TAggregatedValue> aggregator)
        {
            return Add(new ComplexLogProperty<TValue, TAggregatedValue>(containerName, propertyName, propertyValue, aggregator));
        }
    }
}