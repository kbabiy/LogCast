using System;
using JetBrains.Annotations;

namespace LogCast.Loggers.Direct
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class DirectLoggerOptions
    {
        /// <summary>
        /// Required. Verbosity level of the logs accepted
        /// </summary>
        public LogLevel MinLogLevel { get; }

        /// <summary>
        /// Required. Logging application identifier
        /// </summary>
        public string SystemType { get; }

        /// <summary>
        /// Layout to arrange messages in the raw Unit of Work view (@fields.details)
        /// </summary>
        public string Layout { get; set; }

        /// <summary>
        /// Where the fallback logs (in case logging system is inaccessible or other failutes) are written
        /// </summary>
        public string FallbackLogDirectory { get; set; }

        /// <summary>
        /// Number of days after which fallback logs are auto cleaned up
        /// </summary>
        public int DaysToKeepFallbackLogs { get; set; }

        /// <summary>
        /// This is an option to decrease amount of logs sent from this application by certain percentage skipped
        /// </summary>
        public int SkipPercentage { get; set; }

        public DirectLoggerOptions(LogLevel minLogLevel, string systemType)
        {
            if (string.IsNullOrWhiteSpace(systemType))
                throw new ArgumentNullException(nameof(systemType));

            MinLogLevel = minLogLevel;
            SystemType = systemType;
        }
    }
}
