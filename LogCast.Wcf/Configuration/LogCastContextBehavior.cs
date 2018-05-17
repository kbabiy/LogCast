using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using JetBrains.Annotations;

namespace LogCast.Wcf.Configuration
{
    public enum LoggingOptionsPreset
    {
        None,
        RequestOnly,
        ResponseOnly,
        All
    }

    /// <summary>
    /// Registers a WCF message inspector which opens a <see cref="LogCastContext"/> for each service call. 
    /// Additionally, information from request/response message is logged according to the logging flags set.
    /// This behavior can be applied either through config file or as a service attribute.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    [AttributeUsage(AttributeTargets.Class)]
    public class LogCastContextBehavior : Attribute, IServiceBehavior, IConfigurableOptions
    {
        public bool LogCallerAddress { get; set; }
        public bool LogRequestProperties { get; set; }
        public bool LogRequestHttpData { get; set; }
        public bool LogRequestBody { get; set; }

        public bool LogResponseProperties { get; set; }
        public bool LogResponseHttpData { get; set; }
        public bool LogResponseBody { get; set; }

        public LoggingOptionsPreset LoggingPreset { get; set; }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            LoggingOptions options;
            if (LoggingPreset == LoggingOptionsPreset.None)
            {
                options = new LoggingOptions();
                LoggingOptions.Copy(this, options);
            }
            else
            {
                options = FromPreset(LoggingPreset);
            }

            var inspector = new LogCastContextInspector(null, options);
            foreach (var channel in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>())
            {
                foreach (EndpointDispatcher endpoint in channel.Endpoints)
                {
                    endpoint.DispatchRuntime.MessageInspectors.Add(inspector);
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { }

        public void AddBindingParameters(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        { }

        private static LoggingOptions FromPreset(LoggingOptionsPreset preset)
        {
            LoggingOptions options;
            switch (preset)
            {
                case LoggingOptionsPreset.RequestOnly:
                    options = LoggingOptions.RequestOnly();
                    break;
                case LoggingOptionsPreset.ResponseOnly:
                    options = LoggingOptions.ResponseOnly();
                    break;
                case LoggingOptionsPreset.All:
                    options = LoggingOptions.All();
                    break;
                default:
                    options = new LoggingOptions();
                    break;
            }

            return options;
        }
    }
}