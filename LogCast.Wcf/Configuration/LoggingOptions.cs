using JetBrains.Annotations;

namespace LogCast.Wcf.Configuration
{
    // We do not log message body because it can be accessed only ONCE thus we'd have to clone it first which looks like 
    // a significant overhead (see https://msdn.microsoft.com/en-us/library/ms734675.aspx)
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class LoggingOptions : IConfigurableOptions
    {
        public bool LogCallerAddress { get; set; }
        public bool LogRequestProperties { get; set; }
        public bool LogRequestHttpData { get; set; }
        public bool LogRequestBody { get; set; }

        internal bool IsRequestLogged => 
            LogCallerAddress || 
            LogRequestProperties || 
            LogRequestHttpData || 
            LogRequestBody;


        public bool LogResponseProperties { get; set; }
        public bool LogResponseHttpData { get; set; }
        /// <summary>
        /// Not supported by <see cref="OperationContextInspector"/>
        /// </summary>
        public bool LogResponseBody { get; set; }

        internal bool IsResponseLogged =>
            LogResponseProperties ||
            LogResponseHttpData ||
            LogResponseBody;

        public static LoggingOptions All()
        {
            var options = new LoggingOptions
            {
                LogRequestBody = true,
                LogResponseBody = true
            };
            options.EnableRequestOptions();
            options.EnableResponseOptions();

            return options;
        }

        public static LoggingOptions RequestOnly()
        {
            var options = new LoggingOptions();
            options.EnableRequestOptions();

            return options;
        }

        public static LoggingOptions ResponseOnly()
        {
            var options = new LoggingOptions();
            options.EnableResponseOptions();

            return options;
        }

        private void EnableRequestOptions()
        {
            LogCallerAddress = true;
            LogRequestProperties = true;
            LogRequestHttpData = true;
        }

        private void EnableResponseOptions()
        {
            LogResponseProperties = true;
            LogResponseHttpData = true;
        }

        internal LoggingOptions Clone()
        {
            var options = new LoggingOptions();
            Copy(this, options);

            return options;
        }

        internal static void Copy(IConfigurableOptions source, IConfigurableOptions target)
        {
            target.LogCallerAddress = source.LogCallerAddress;
            target.LogRequestProperties = source.LogRequestProperties;
            target.LogRequestHttpData = source.LogRequestHttpData;
            target.LogRequestBody = source.LogRequestBody;

            target.LogResponseProperties = source.LogResponseProperties;
            target.LogResponseHttpData = source.LogResponseHttpData;
            target.LogResponseBody = source.LogResponseBody;
        }
    }
}
