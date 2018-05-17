using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace LogCast.Engine
{
    /// <summary>
    /// A document that contains data to send to centralizd log server
    /// </summary>
    public class LogCastDocument
    {
        private const char TopLevelPropertiesPrefix = '@';

        private readonly Dictionary<string, object> _topLevelProperties;
        private readonly Dictionary<string, object> _customProperties;

        public static readonly StringComparer FieldNamesComparer = StringComparer.OrdinalIgnoreCase;

        public LogCastDocument()
        {
            _customProperties = new Dictionary<string, object>(FieldNamesComparer);
            _topLevelProperties = new Dictionary<string, object>(FieldNamesComparer);
        }

        public void AddProperty<T>(string parentName, string childName, T value, bool suppressDefaults = false)
        {
            if (string.IsNullOrEmpty(parentName))
                throw new ArgumentNullException(nameof(parentName));
            if (string.IsNullOrEmpty(childName))
                throw new ArgumentNullException(nameof(childName));

            if (ShouldSet(suppressDefaults, value))
            {
                var container = GetContainer(parentName, true);
                container[childName] = value;
            }
        }

        public void AddProperty<T>(string propertyName, T value, bool suppressDefaults = false)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (ShouldSet(suppressDefaults, value))
            {
                var container = GetRoot(propertyName);
                container[propertyName] = value;
            }
        }

        public T GetProperty<T>(string propertyName)
        {
            var container = GetRoot(propertyName);
            return Get<T>(container, propertyName);
        }

        public T GetProperty<T>(string parentName, string childName)
        {
            var container = GetContainer(parentName, false);
            return Get<T>(container, childName);
        }

        public bool PropertyExists(string propertyName)
        {
            var container = GetRoot(propertyName);
            return container.ContainsKey(propertyName);
        }

        public string ToJson()
        {
            if (_customProperties.Count > 0)
                _topLevelProperties[Property.Root] = _customProperties;

            return ToJson(_topLevelProperties);
        }

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            Culture = CultureInfo.InvariantCulture
        };

        public static string ToJson(object what)
        {
            return JsonConvert.SerializeObject(what, Settings);
        }

        private Dictionary<string, object> GetContainer(string parentName, bool createIfMissing)
        {
            var root = GetRoot(parentName);
            var result = Get<Dictionary<string, object>>(root, parentName);
            if (createIfMissing && result == null)
            {
                result = new Dictionary<string, object>();
                root[parentName] = result;
            }
            return result;
        }

        private Dictionary<string, object> GetRoot(string propertyName)
        {
            return propertyName?.First() == TopLevelPropertiesPrefix ? _topLevelProperties : _customProperties;
        }

        private static T Get<T>(Dictionary<string, object> container, string key)
        {
            return container != null && container.TryGetValue(key, out var value)
                ? (T) value
                : default(T);
        }

        private static bool ShouldSet<T>(bool suppressDefaults, T value)
        {
            return !suppressDefaults || !EqualityComparer<T>.Default.Equals(value, default(T));
        }
    }
}