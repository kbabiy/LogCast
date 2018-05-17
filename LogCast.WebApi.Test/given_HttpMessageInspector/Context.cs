using LogCast.Http;

namespace LogCast.WebApi.Test.given_HttpMessageInspector
{
    public abstract class Context : Test.Context
    {
        protected virtual LoggingOptions Options => new LoggingOptions();
        protected HttpMessageInspector Sut;

        protected LogCastContext LogCastContext;

        public override void Arrange()
        {
            base.Arrange();
            LogCastContext = new LogCastContext();
            Sut = new HttpMessageInspector(Options);            
        }

        public override void Cleanup()
        {
            LogCastContext.Dispose();
        }
    }
}