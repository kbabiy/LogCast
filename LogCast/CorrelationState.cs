namespace LogCast
{
    public class CorrelationState<TRoot>
    {
        public const string Key = "CorrelationState";
        public LogCastContext Context { get; }
        public TRoot LogEntry { get; }

        public CorrelationState(LogCastContext context, TRoot logEntry)
        {
            Context = context;
            LogEntry = logEntry;
        }
    }
}