using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using JetBrains.Annotations;

namespace LogCast.Wcf.Configuration
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class LogCastContextBehaviorElement : BehaviorExtensionElement, IConfigurableOptions
    {
        [ConfigurationProperty("logCallerAddress")]
        public bool LogCallerAddress
        {
            get => (bool)base["logCallerAddress"];
            set => base["logCallerAddress"] = value;
        }

        [ConfigurationProperty("logRequestProperties")]
        public bool LogRequestProperties
        {
            get => (bool)base["logRequestProperties"];
            set => base["logRequestProperties"] = value;
        }

        [ConfigurationProperty("logRequestHttpData")]
        public bool LogRequestHttpData
        {
            get => (bool)base["logRequestHttpData"];
            set => base["logRequestHttpData"] = value;
        }

        [ConfigurationProperty("logRequestBody")]
        public bool LogRequestBody
        {
            get => (bool)base["logRequestBody"];
            set => base["logRequestBody"] = value;
        }

        [ConfigurationProperty("logResponseProperties")]
        public bool LogResponseProperties
        {
            get => (bool)base["logResponseProperties"];
            set => base["logResponseProperties"] = value;
        }

        [ConfigurationProperty("logResponseHttpData")]
        public bool LogResponseHttpData
        {
            get => (bool)base["logResponseHttpData"];
            set => base["logResponseHttpData"] = value;
        }

        [ConfigurationProperty("logResponseBody")]
        public bool LogResponseBody
        {
            get => (bool)base["logResponseBody"];
            set => base["logResponseBody"] = value;
        }

        [ConfigurationProperty("loggingPreset")]
        public LoggingOptionsPreset LoggingPreset
        {
            get => (LoggingOptionsPreset)base["loggingPreset"];
            set => base["loggingPreset"] = value;
        }

        public override Type BehaviorType => typeof(LogCastContextBehavior);

        protected override object CreateBehavior()
        {
            var behavior = new LogCastContextBehavior { LoggingPreset = LoggingPreset };
            LoggingOptions.Copy(this, behavior);

            return behavior;
        }
    }
}