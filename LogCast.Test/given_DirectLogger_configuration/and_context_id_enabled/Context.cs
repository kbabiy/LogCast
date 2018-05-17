namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public abstract class Context : given_DirectLogger_configuration.Context
    {
        protected const string OperationName = "test-operation";
        protected LogCastContext CurrentContext;
        protected virtual string CorrelationId => "test-correlation-id";

        public override void Arrange()
        {
            base.Arrange();
            CurrentContext = new LogCastContext(CorrelationId)
            {
                OperationName = OperationName
            };
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