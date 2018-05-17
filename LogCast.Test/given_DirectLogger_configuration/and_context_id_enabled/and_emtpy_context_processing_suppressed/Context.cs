namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled.and_emtpy_context_processing_suppressed
{
    public abstract class Context : and_context_id_enabled.Context
    {
        public override void Arrange()
        {
            base.Arrange();
            CurrentContext.SuppressEmtpyContextMessages = true;
        }

        public override void Act()
        {
            Complete();
        }
    }
}