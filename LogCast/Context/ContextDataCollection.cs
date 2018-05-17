using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace LogCast.Context
{
    /// <summary>
    /// A collection for storing context-specific data. All public methods of this collection are thread-safe.
    /// Enumeration is not thread-safe and must be done after the collection entered the read-only mode.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class ContextDataCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new List<T>();
        private readonly object _syncRoot = new object();

        public bool IsReadOnly { get; private set; }
        public bool IsFull => MaxCapacity >= 0 && _items.Count >= MaxCapacity;
        public bool IsEmpty => _items.Count == 0;

        /// <summary>
        /// Maximum number of allowed elements
        /// </summary>
        public int MaxCapacity { get; }

        public ContextDataCollection()
        {
            MaxCapacity = -1;
        }

        public ContextDataCollection(int maxCapacity)
        {
            MaxCapacity = maxCapacity;
        }

        /// <summary>
        /// Adds a specified element to the collection. This method is thread-safe
        /// </summary>
        /// <returns> Returns 'true' if the element has been added to the collection. May return 
        /// 'False' if the collection entered the read-only mode or maximum capacity reached.
        /// </returns>
        public bool Add(T item)
        {
            lock (_syncRoot)
            {
                if (IsReadOnly || IsFull)
                    return false;

                _items.Add(item);
                return true;
            }
        }

        /// <summary>
        /// Adds specified elements to the collection. This method is thread-safe
        /// </summary>
        /// <returns> Returns actual number of the added elements. 
        /// May return a number which is less then the number of the specified elements if maximum capacity reached.
        /// May return zero if the collection entered the read-only mode.
        /// </returns>
        public int AddRange(IEnumerable<T> items)
        {
            if (items == null)
                return 0;

            lock (_syncRoot)
            {
                if (IsReadOnly || IsFull)
                    return 0;

                int initialCount = _items.Count;
                _items.AddRange(MaxCapacity < 0
                    ? items
                    : items.Take(MaxCapacity - initialCount));

                return _items.Count - initialCount;
            }
        }

        internal void SetReadOnly()
        {
            lock (_syncRoot)
            {
                IsReadOnly = true;
            }
        }

        /// <summary>
        /// Enumeration is not thread-safe and must be done after the collection entered the read-only mode.
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
