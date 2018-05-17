using BddStyle.NUnit;

namespace LogCast.Test.given_LogCastContextBranch
{
    public abstract class Context : ContextBase
    {
        protected LogCastContextBranch CurrentBranch => LogCastContextBranch.Current;
    }
}