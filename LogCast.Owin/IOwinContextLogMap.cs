namespace LogCast.Owin
{
    public interface IOwinContextLogMap
    {
        void AfterNextHandler(LogCastContext logContext);
    }
}
