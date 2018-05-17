using System.Diagnostics.CodeAnalysis;
using LogCast.Engine;

namespace LogCast.Inspectors
{
    public class ConfigurationInspector : ILogDispatchInspector
    {
        public string AppName { get;  }
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public string LoggingFramework { get; }

        public ConfigurationInspector(string appName, string loggingFramework)
        {
            AppName = appName;
            LoggingFramework = loggingFramework;
        }

        public void BeforeSend(LogCastDocument document, LogMessage sourceMessage)
        {
            Apply(document);
        }

        public void BeforeSend(LogCastDocument document, LogCastContext sourceContext)
        {
            Apply(document);
        }

        private void Apply(LogCastDocument document)
        {
            document.AddProperty(Property.AppName, AppName, true);
            document.AddProperty(Property.LoggingFramework, LoggingFramework, true);
        }
    }
}