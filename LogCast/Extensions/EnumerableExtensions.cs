using System.Collections.Generic;

namespace LogCast.Extensions
{
    public static class EnumerableExtensions
    {
        public static T[] InsertBetween<T>(
            this IEnumerable<T> source, int length, T extraElement)
        {
            var newLength = length * 2 - 1;
            var result = new T[newLength];
            using (var enumerator = source.GetEnumerator())
            {
                for (int i = 0; i < length - 1; i++)
                {
                    enumerator.MoveNext();
                    result[i * 2] = enumerator.Current;
                    result[i * 2 + 1] = extraElement;
                }
                enumerator.MoveNext();
                result[newLength - 1] = enumerator.Current;
            }
            return result;
        }
    }
}