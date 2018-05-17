namespace LogCast.NLog.Test.LogCastTarget.given_nlog_configuration.and_context_id_enabled
{
    public abstract class Context : given_nlog_configuration.Context
    {
        protected const string OperationName = "TestOperationName";
        protected LogCastContext CurrentContext;
        protected const string CorrelationId = "TestCorrelationId";

        public override void Arrange()
        {
            base.Arrange();
            CurrentContext = new LogCastContext(CorrelationId) {OperationName = OperationName};
        }

        protected void Complete()
        {
            CurrentContext?.Dispose();
        }

        public override void Cleanup()
        {
            Complete();
            base.Cleanup();
        }
    }
}