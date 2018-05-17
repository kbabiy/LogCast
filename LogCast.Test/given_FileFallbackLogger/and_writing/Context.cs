namespace LogCast.Test.given_FileFallbackLogger.and_writing
{
    public abstract class Context : given_FileFallbackLogger.Context
    {
        public override void Act()
        {
            Write("message");
        }

        protected void Write(string message)
        {
            SutMock.Object.Write(message);
        }
    }
}