using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace LogCast.Utilities
{
    public class EnvironmentContext
    {
        private static readonly Lazy<Assembly> EntryAssembly = new Lazy<Assembly>(GetEntryAssembly);
        private static readonly Lazy<Version> LibraryVersion = new Lazy<Version>(GetLoggingLibraryVersion);

        public string GetHostName()
        {
            return Environment.MachineName;
        }

        public IPAddress[] GetHostIps()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .ToArray();
        }

        public Version GetLibraryVersion()
        {
            return LibraryVersion.Value;
        }

        public Version GetAppVersion()
        {
            return EntryAssembly.Value?.GetName().Version;
        }

        internal static bool IsUnitTestContext()
        {
            return EntryAssembly.Value == null;
        }

        // NOTE: ensure this method doesn't return unit tests runner assemblies (see IsUnitTestContext() method)
        private static Assembly GetEntryAssembly()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly() ?? GetWebApiEntryAssembly();
                return assembly;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return null;
        }

        private static Assembly GetWebApiEntryAssembly()
        {
            //Below code is to execute next chain with late binding to System.Web:
            //var type = System.Web.HttpContext.Current?.ApplicationInstance?.GetType();
            var systemWebAsm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.StartsWith("System.Web,"));

            var currentContext = systemWebAsm
                ?.GetType("System.Web.HttpContext")
                ?.GetProperty("Current", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                ?.GetValue(null);

            var httpApplication = currentContext
                ?.GetType()
                .GetProperty("ApplicationInstance", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                ?.GetValue(currentContext);

            var type = httpApplication
                ?.GetType();

            while (type != null && type.Namespace == "ASP")
                type = type.BaseType;

            var entryAssembly = type?.Assembly;

            return entryAssembly;
        }

        private static Version GetLoggingLibraryVersion()
        {
            return typeof(ILogger).Assembly.GetName().Version;
        }
    }
}