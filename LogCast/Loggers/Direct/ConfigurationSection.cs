using System.Configuration;
using JetBrains.Annotations;

namespace LogCast.Loggers.Direct
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class ConfigurationSection : System.Configuration.ConfigurationSection
    {
        public static ConfigurationSection Read()
        {
            const string sectionName = "logCast";
            var result = (ConfigurationSection)ConfigurationManager.GetSection(sectionName);
            if (result == null)
                throw new ConfigurationErrorsException($"Did not find configuraiton section '{sectionName}'");

            return result;
        }

        private string Get(string key)
        {
            return Get<string>(key);
        }

        private T Get<T>(string key)
        {
            return (T)this[key];
        }

        [ConfigurationProperty("endpoint", IsRequired = true)]
        public string Endpoint => Get("endpoint");

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type => Get("type");

        [ConfigurationProperty("logLevel", IsRequired = true)]
        public string LogLevel => Get("logLevel");

        [ConfigurationProperty("layout", IsRequired = false)]
        public string Layout => Get("layout");

        [ConfigurationProperty("throttling", IsRequired = false)]
        public string Throttling => Get("throttling");

        [ConfigurationProperty("sendingThreadCount", IsRequired = false)]
        public string SendingThreadCount => Get("sendingThreadCount");

        [ConfigurationProperty("enableSelfDiagnostics", IsRequired = false)]
        public string EnableSelfDiagnostics => Get("enableSelfDiagnostics");

        [ConfigurationProperty("fallbackLogDirectory", IsRequired = false)]
        public string FallbackLogDirectory => Get("fallbackLogDirectory");

        [ConfigurationProperty("daysToKeepFallbackLogs", IsRequired = false)]
        public int DaysToKeepFallbackLogs => Get<int>("daysToKeepFallbackLogs");
    }
}