namespace LogCast
{
    public static class Property
    {
        public const string AppName = "@type";
        public const string LoggingFramework = "@source";
        public const string Timestamp = "@timestamp";
        public const string Message = "@message";

        public const string Root = "@fields";

        public const string Host = "host";
        public const string AppVersion = "application_version";
        public const string CorrelationId = "correlation_id";
        public const string LoggerName = "logger";
        public const string OperationName = "operation";
        public const string LogLevel = "log_level";
        public const string LogLevelCode = "log_level_code";
        public const string Details = "details";

        public const string Exceptions = "exceptions";

        public const string DefaultChildName = "value";

        public static class Durations
        {
            public const string Name = "durations";
            public const string Total = "total";
        }
        
        public static class Logging
        {
            public const string Name = "logs";
            
            public const string LibVersion = "version";
            public const string DropCount = "drop_count";
            public const string RetryCount = "retry_count";
            public const string MessageLength = "length";

            //Only set when diagnostics is init
            public const string DeliveryDelay = "delivery_delay";
            public const string CreationTime = "creation_time";
        }

        public static class Values
        {
            public const string None = "<none>";
            public const string MediaDataPlaceholder = "<media>";
            public const string Removed = "<removed>";
        }
    }
}