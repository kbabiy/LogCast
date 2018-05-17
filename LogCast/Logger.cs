using LogCast.Utilities;
using System;
using System.Collections.Generic;
using LogCast.Data;
using LogCast.Loggers;

namespace LogCast
{
    internal class Logger : ILogger
    {
        private readonly object _syncLock = new object();
        private volatile ILoggerBridge _bridge;
        private Func<string, ILoggerBridge> _bridgeFactory;

        // We get bridgeFactory to postpone possible initialization of the actual logging framework
        // which should take place AFTER the configuration only.

        public Logger(string name, Func<string, ILoggerBridge> bridgeFactory)
        {
            Name = name;
            _bridgeFactory = bridgeFactory ?? throw new ArgumentNullException(nameof(bridgeFactory));
        }

        public string Name { get; }
        public bool IsTraceEnabled => Bridge.IsLevelEnabled(LogLevel.Trace);
        public bool IsDebugEnabled => Bridge.IsLevelEnabled(LogLevel.Debug);
        public bool IsInfoEnabled => Bridge.IsLevelEnabled(LogLevel.Info);
        public bool IsWarnEnabled => Bridge.IsLevelEnabled(LogLevel.Warn);
        public bool IsErrorEnabled => Bridge.IsLevelEnabled(LogLevel.Error);
        public bool IsFatalEnabled => Bridge.IsLevelEnabled(LogLevel.Fatal);

        public bool AddContextProperty<T>(string propertyName, T propertyValue)
        {
            return AddContextProperties(new LogProperty<T>(propertyName, propertyValue));
        }

        public bool AddContextProperty<TValue, TAggregatedValue>(string propertyName, TValue propertyValue,
            Func<IEnumerable<TValue>, TAggregatedValue> aggregator)
        {
            return AddContextProperties(new LogProperty<TValue, TAggregatedValue>(propertyName, propertyValue, aggregator));
        }

        public bool AddContextProperty<T>(string containerName, string propertyName, T propertyValue)
        {
            return AddContextProperties(new ComplexLogProperty<T>(containerName, propertyName, propertyValue));
        }

        public bool AddContextProperty<TValue, TAggregatedValue>(string containerName, string propertyName,
            TValue propertyValue, Func<IEnumerable<TValue>, TAggregatedValue> aggregator)
        {
            return AddContextProperties(new ComplexLogProperty<TValue, TAggregatedValue>(containerName, propertyName, propertyValue, aggregator));
        }

        public bool AddContextProperties(params LogProperty[] properties)
        {
            var context = LogCastContext.Current;
            if (context == null)
                return false;

            if (properties.Length > 0)
                context.Properties.AddRange(properties);

            return true;
        }

        private ILoggerBridge Bridge
        {
            get
            {
                if (_bridge != null)
                    return _bridge;

                lock (_syncLock)
                {
                    if (_bridge == null)
                    {
                        // If factory throws an exceptino we still have a chance to try on next logging call
                        _bridge = _bridgeFactory(Name);
                        _bridgeFactory = null;
                    }
                }

                return _bridge;
            }
        }

        private void Log(LogLevel level, string format, object[] args, LogProperty[] properties)
        {
            Log(level, FormatHelper.FormatMessage(format, args), (Exception)null, properties);
        }

        private void Log(LogLevel level, string message, Exception exception, LogProperty[] properties)
        {
            if (string.IsNullOrEmpty(message) && exception != null)
                message = $"{exception.GetType().FullName}: {exception.Message}";

            Bridge.Log(level, message, exception, properties);
        }

        #region Trace

        public void Trace(string format, params object[] args)
        {
            Log(LogLevel.Trace, format, args, null);
        }

        public void Trace(LogProperty[] properties, string format, params object[] args)
        {
            Log(LogLevel.Trace, format, args, properties);
        }
        #endregion

        #region Debug
        public void Debug(string format, params object[] args)
        {
            Log(LogLevel.Debug, format, args, null);
        }

        public void Debug(LogProperty[] properties, string format, params object[] args)
        {
            Log(LogLevel.Debug, format, args, properties);
        }
        #endregion

        #region Info
        public void Info(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args, null);
        }

        public void Info(LogProperty property, string format, params object[] args)
        {
            Log(LogLevel.Info, format, args, new[] { property });
        }

        public void Info(LogProperty[] properties, string format, params object[] args)
        {
            Log(LogLevel.Info, format, args, properties);
        }
        #endregion

        #region Warn
        public void Warn(string format, params object[] args)
        {
            Log(LogLevel.Warn, format, args, null);
        }

        public void Warn(LogProperty[] properties, string format, params object[] args)
        {
            Log(LogLevel.Warn, format, args, properties);
        }

        public void Warn(Exception exception)
        {
            Log(LogLevel.Warn, null, exception, null);
        }

        public void Warn(string message, Exception exception)
        {
            Log(LogLevel.Warn, message, exception, null);
        }

        public void Warn(LogProperty[] properties, string message, Exception exception)
        {
            Log(LogLevel.Warn, message, exception, properties);
        }

        #endregion

        #region Error
        public void Error(string format, params object[] args)
        {
            Log(LogLevel.Error, format, args, null);
        }

        public void Error(LogProperty[] properties, string format, params object[] args)
        {
            Log(LogLevel.Error, format, args, properties);
        }

        public void Error(Exception exception)
        {
            Log(LogLevel.Error, null, exception, null);
        }

        public void Error(string message, Exception exception)
        {
            Log(LogLevel.Error, message, exception, null);
        }

        public void Error(LogProperty[] properties, string message, Exception exception)
        {
            Log(LogLevel.Error, message, exception, properties);
        }
        #endregion

        #region Fatal
        public void Fatal(string format, params object[] args)
        {
            Log(LogLevel.Fatal, format, args, null);
        }

        public void Fatal(LogProperty[] properties, string format, params object[] args)
        {
            Log(LogLevel.Fatal, format, args, properties);
        }

        public void Fatal(Exception exception)
        {
            Log(LogLevel.Fatal, null, exception, null);
        }

        public void Fatal(string message, Exception exception)
        {
            Log(LogLevel.Fatal, message, exception, null);
        }

        public void Fatal(LogProperty[] properties, string message, Exception exception)
        {
            Log(LogLevel.Fatal, message, exception, properties);
        }
        #endregion

    }
}
