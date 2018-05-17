namespace LogCast.Wcf.Configuration
{
    // We need this interface to ensure that logging options are consistent across all configuration points: 
    // LogCastContextBehavior, LogCastContextBehaviorElement, and LoggingOptions
    internal interface IConfigurableOptions
    {
        bool LogCallerAddress { get; set; }
        bool LogRequestProperties { get; set; }
        bool LogRequestHttpData { get; set; }
        bool LogRequestBody { get; set; }

        bool LogResponseProperties { get; set; }
        bool LogResponseHttpData { get; set; }
        bool LogResponseBody { get; set; }
    }
}
