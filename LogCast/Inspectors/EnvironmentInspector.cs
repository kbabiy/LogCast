using System.Linq;
using LogCast.Engine;
using LogCast.Utilities;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace LogCast.Inspectors
{
    public class EnvironmentInspector : ILogDispatchInspector
    {
        private readonly HostData _hostData;
        private readonly string _appVersion;
        private readonly string _libVersion;

        public EnvironmentInspector(EnvironmentContext context)
        {
            _hostData = new HostData
            {
                HostName = context.GetHostName(),
                HostIps = context.GetHostIps().Select(x => x.ToString()).ToArray()
            };
            _appVersion = context.GetAppVersion()?.ToString();
            _libVersion = context.GetLibraryVersion()?.ToString();
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
            document.AddProperty(Property.Host, _hostData);
            document.AddProperty(Property.AppVersion, _appVersion);
            document.AddProperty(Property.Logging.Name, Property.Logging.LibVersion, _libVersion);
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private class HostData
        {
            [JsonProperty(PropertyName = "name")]
            public string HostName;

            [JsonProperty(PropertyName = "ip")]
            public string[] HostIps;
        }
    }
}
