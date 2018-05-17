using JetBrains.Annotations;

namespace LogCast.Http
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class LoggingOptions
    {
        public bool AllowSensitiveData { get; set; }

        public bool LogRequestUri { get; set; }
        public bool LogRequestMethod { get; set; }
        public bool LogRequestHeaders { get; set; }
        public bool LogRequestClaims { get; set; }
        public bool LogRequestRouteInfo { get; set; }
        public bool LogRequestBody { get; set; }

        public bool IsRequestLogged =>
            LogRequestUri ||
            LogRequestMethod ||
            LogRequestHeaders ||
            LogRequestClaims ||
            LogRequestRouteInfo ||
            LogRequestBody;

        public bool LogResponseStatus { get; set; }
        public bool LogResponseHeaders { get; set; }
        public bool LogResponseBody { get; set; }


        public bool IsResponseLogged =>
            LogResponseStatus ||
            LogResponseHeaders ||
            LogResponseBody;

        public static LoggingOptions Default()
        {
            var options = new LoggingOptions();
            options.EnableRequestOptions();
            options.EnableResponseOptions();
            return options;
        }

        public static LoggingOptions All()
        {
            var options = Default();
            options.LogRequestBody = true;
            options.LogResponseBody = true;
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
            LogRequestUri = true;
            LogRequestMethod = true;
            LogRequestHeaders = true;
            LogRequestClaims = true;
            LogRequestRouteInfo = true;
        }

        private void EnableResponseOptions()
        {
            LogResponseStatus = true;
            LogResponseHeaders = true;
        }

        internal LoggingOptions Clone()
        {
            return new LoggingOptions
            {
                AllowSensitiveData = AllowSensitiveData,

                LogRequestUri = LogRequestUri,
                LogRequestMethod = LogRequestMethod,
                LogRequestHeaders = LogRequestHeaders,
                LogRequestClaims = LogRequestClaims,
                LogRequestRouteInfo = LogRequestRouteInfo,
                LogRequestBody = LogRequestBody,

                LogResponseStatus = LogResponseStatus,
                LogResponseHeaders = LogResponseHeaders,
                LogResponseBody = LogResponseBody
            };
        }
    }
}