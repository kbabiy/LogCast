using System;
using System.Collections.Generic;
using LogCast.Data;
using JetBrains.Annotations;

namespace LogCast
{
    /// <summary>
    /// Generic interface used for logging. 
    /// All public methods of the actual implementor are thread-safe.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public interface ILogger
    {
        string Name { get; }

        [StringFormatMethod("format")]
        void Trace(string format, params object[] args);
        [StringFormatMethod("format")]
        void Debug(string format, params object[] args);
        [StringFormatMethod("format")]
        void Info(string format, params object[] args);
        [StringFormatMethod("format")]
        void Warn(string format, params object[] args);
        [StringFormatMethod("format")]
        void Error(string format, params object[] args);
        [StringFormatMethod("format")]
        void Fatal(string format, params object[] args);

        [StringFormatMethod("format")]
        void Trace(LogProperty[] properties, string format, params object[] args);
        [StringFormatMethod("format")]
        void Debug(LogProperty[] properties, string format, params object[] args);
        [StringFormatMethod("format")]
        void Info(LogProperty[] properties, string format, params object[] args);
        [StringFormatMethod("format")]
        void Warn(LogProperty[] properties, string format, params object[] args);
        [StringFormatMethod("format")]
        void Error(LogProperty[] properties, string format, params object[] args);
        [StringFormatMethod("format")]
        void Fatal(LogProperty[] properties, string format, params object[] args);

        // Make additional overload for Info because it may be used quite often
        [StringFormatMethod("format")]
        void Info(LogProperty property, string format, params object[] args);

        // Not always an exception is application's error.
        void Warn(Exception exception);
        void Warn(string message, Exception exception);
        void Warn(LogProperty[] properties, string message, Exception exception);

        void Error(Exception exception);
        void Error(string message, Exception exception);
        void Error(LogProperty[] properties, string message, Exception exception);

        void Fatal(Exception exception);
        void Fatal(string message, Exception exception);
        void Fatal(LogProperty[] properties, string message, Exception exception);

        bool IsTraceEnabled { get; }
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }

        bool AddContextProperty<T>(string propertyName, T propertyValue);
        bool AddContextProperty<TValue, TAggregatedValue>(string propertyName, TValue propertyValue,
            Func<IEnumerable<TValue>, TAggregatedValue> aggregator);

        bool AddContextProperty<T>(string containerName, string propertyName, T propertyValue);
        bool AddContextProperty<TValue, TAggregatedValue>(string containerName, string propertyName, TValue propertyValue,
            Func<IEnumerable<TValue>, TAggregatedValue> aggregator);

        bool AddContextProperties(params LogProperty[] properties);
    }
}