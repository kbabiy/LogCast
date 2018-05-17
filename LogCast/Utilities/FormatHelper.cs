using JetBrains.Annotations;

namespace LogCast.Utilities
{
    [UsedImplicitly]
    public class FormatHelper
    {
        public static string FormatMessage(string format, object[] args)
        {
            return args != null && args.Length > 0 ? string.Format(format, args) : format;
        }
    }
}
