using System;
using JetBrains.Annotations;
using NLog;
using NLog.Config;
using NLog.Targets;
using LogCast.Delivery;
using LogCast.Engine;
using LogCast.Fallback;

namespace LogCast.NLog
{
    [Target("LogCast")]
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class LogCastTarget : TargetWithLayout
    {
        #region Target options

        [RequiredParameter]
        public string SystemType { get; set; }

        public string SkipPercentage { get; set; }

        /// <summary>
        /// Specifies a directory to store logs of a fallback logger which is used to log errors thrown inside the logging subsystem itself.
        /// Can be either absolute or relative path.
        /// </summary>
        public string FallbackLogDirectory { get; set; }

        /// <summary>
        /// Number of days after which fallback logs are auto cleaned up
        /// </summary>
        public int DaysToKeepFallbackLogs { get; set; }

        #endregion

        #region LogCastClient options

        /// <summary>
        /// LogCast destination address
        /// </summary>
        [RequiredParameter]
        public string Endpoint { get; set; }

        /// <summary>
        /// Limit of messages queued to send before old messages are dropped
        /// </summary>
        public string Throttling { get; set; }

        /// <summary>
        /// How long will retry logic attempt to send one log message before it is dropped
        /// </summary>
        public string RetryTimeout { get; set; }

        /// <summary>
        /// Consumer thread count
        /// </summary>
        public string SendingThreadCount { get; set; }

        /// <summary>
        /// Time threshold to execute log sending call
        /// </summary>
        public string SendTimeout { get; set; }

        /// <summary>
        /// Specifies, whether the target should log additional fields that help profile and troubleshoot the logging subsystem
        /// </summary>
        public string EnableSelfDiagnostics { get; set; }

        #endregion

        public IFallbackLogger FallbackLogger { get; set; }
        public LogCastOptions Options { get; set; }

        private LogMessageRouter _messageRouter;

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            Initialize();
        }

        private void Initialize()
        {
            FallbackLogger = CreateFallbackLogger();
            Options = LogCastOptions.Parse(
                Endpoint, Throttling, RetryTimeout, SendingThreadCount, SendTimeout, EnableSelfDiagnostics);

            _messageRouter = new LogMessageRouter(ParseSkipPercentage(SkipPercentage));

            // This case is mostly for scenario when a client uses "real" NLog LogManager 
            // and doesn't use our ILogger/LogManager/LogConfig
            if (!LogConfig.IsConfigured && !LogConfig.IsInProgress)
            {
                LogConfig.Configure(new NLogManager(this));
            }
        }

        protected virtual IFallbackLogger CreateFallbackLogger()
        {
            return new FileFallbackLogger(FallbackLogDirectory, DaysToKeepFallbackLogs);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            try
            {
                var message = CreateMessage(logEvent);
                _messageRouter.DispatchMessage(message);
            }
            catch (Exception ex)
            {
                FallbackLogger.Write(ex,
                    $"Failed to dispatch a message. Original message: [{logEvent.LoggerName}] {logEvent.Message}");
            }
        }

        private static int ParseSkipPercentage(string skipPercentage)
        {
            int.TryParse(skipPercentage, out var result);
            return result;
        }

        private LogMessage CreateMessage(LogEventInfo logEvent)
        {
            return new LogMessage
            {
                Level = LogHelper.TranslateLogLevel(logEvent.Level),
                OriginalMessage = logEvent.Message,
                RenderedMessage = Layout?.Render(logEvent),
                LoggerName = logEvent.LoggerName,
                Exception = logEvent.Exception,
                CreatedAt = logEvent.TimeStamp,
                Properties = LogHelper.GetLogProperties(logEvent)
            };
        }
    }
}