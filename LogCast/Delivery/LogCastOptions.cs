using System;
using JetBrains.Annotations;

namespace LogCast.Delivery
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class LogCastOptions
    {
        public LogCastOptions(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentNullException(nameof(endpoint));

            Endpoint = new Uri(endpoint);
            SendingThreadCount = 4;
            SendTimeout = TimeSpan.FromSeconds(20);
            Throttling = 10000;
            RetryTimeout = TimeSpan.FromMinutes(5);
            EnableSelfDiagnostics = false;
        }

        public static LogCastOptions Parse(string endpoint, string throttling, string retryTimeout,
            string sendingThreadCount, string sendTimeout, string enableSelfDiagnostics)
        {
            var options = new LogCastOptions(endpoint);

            if (!string.IsNullOrWhiteSpace(throttling))
                options.Throttling = int.Parse(throttling);
            if (!string.IsNullOrWhiteSpace(retryTimeout))
                options.RetryTimeout = TimeSpan.Parse(retryTimeout);
            if (!string.IsNullOrWhiteSpace(sendingThreadCount))
                options.SendingThreadCount = int.Parse(sendingThreadCount);
            if (!string.IsNullOrWhiteSpace(sendTimeout))
                options.SendTimeout = TimeSpan.Parse(sendTimeout);
            if (!string.IsNullOrWhiteSpace(enableSelfDiagnostics))
                options.EnableSelfDiagnostics = ToBool(enableSelfDiagnostics);

            return options;
        }

        /// <summary>
        /// Required. Logs destination address
        /// </summary>
        public Uri Endpoint { get; private set; }

        /// <summary>
        /// Consumer thread count
        /// </summary>
        public int SendingThreadCount { get; set; }

        /// <summary>
        /// Time threshold to execute log sending call
        /// </summary>
        public TimeSpan SendTimeout { get; set; }

        /// <summary>
        /// How long will retry logic attempt to send one log message before it is dropped
        /// </summary>
        public TimeSpan RetryTimeout { get; set; }

        /// <summary>
        /// Limit of messages queued to send before old messages are dropped
        /// </summary>
        public int Throttling { get; set; }

        /// <summary>
        /// Specifies, whether the target should log additional fields that help profile and troubleshoot the logging subsystem
        /// </summary>
        public bool EnableSelfDiagnostics { get; set; }

        private static bool ToBool(string s)
        {
            return bool.TryParse(s, out var result) && result;
        }
    }
}