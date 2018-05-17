namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration.and_context_id_enabled
{
    public abstract class Context : given_configuration.Context
    {
        protected const string TestMessage = "TraceLogging test message";
        protected const string OperationName = "TestOperationNamw";
        protected string InitialCorrelationId;
        private LogCastContext _currentContext;

        public override void Arrange()
        {
            base.Arrange();
            _currentContext = new LogCastContext(InitialCorrelationId) { OperationName = OperationName };
        }

        protected void Complete()
        {
            _currentContext?.Dispose();
        }

        public override void Cleanup()
        {
            Complete();
            base.Cleanup();
        }
    }
}