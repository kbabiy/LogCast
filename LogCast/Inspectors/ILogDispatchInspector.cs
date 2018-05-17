using LogCast.Engine;

namespace LogCast.Inspectors
{
    public interface ILogDispatchInspector
    {
        void BeforeSend(LogCastDocument document, LogMessage sourceMessage);
        void BeforeSend(LogCastDocument document, LogCastContext sourceContext);
    }
}