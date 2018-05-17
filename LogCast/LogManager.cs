using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using LogCast.Loggers;

namespace LogCast
{
    /// <summary>
    /// Use static methods of this class to initialize an instance of a logger
    /// All public methods of this class are thread-safe
    /// </summary>
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public static class LogManager
    {
        private static readonly object FlushLock = new object();

        public static ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T).Name);
        }

        public static ILogger GetLogger(Type type)
        {
            return GetLogger(type?.Name);
        }

        public static ILogger GetLogger([CallerFilePath]string name = null)
        {
            string newName = null;

            try
            {
                if (Path.IsPathRooted(name))
                    newName = Path.GetFileNameWithoutExtension(name);
            }
            catch (ArgumentException)
            {
                newName = name;
            }

            var result = new Logger(newName ?? name, CreateBridge);
            return result;
        }

        public static void Flush(TimeSpan timeout)
        {
            if (!LogConfig.IsConfigured)
                return;

            lock (FlushLock)
            {
                LogConfig.Current.LogManager.Flush(timeout);
                LogConfig.Current.Engine.Flush(timeout);
            }
        }

        private static ILoggerBridge CreateBridge(string name)
        {
            return LogConfig.Current.LogManager.GetLoggerBridge(name);
        }
    }
}
