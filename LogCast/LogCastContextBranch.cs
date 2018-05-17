using System;
using LogCast.Context;

namespace LogCast
{
    /// <summary>
    /// Marks the boundaries of the logical sequential piece of code
    /// Used to meaningfully arrange items from code executed in parralel 
    /// and calculate the statistics (such as durations)
    /// </summary>
    /// <remarks>Should be declared inside LogCastContext only</remarks>
    /// <remarks>Nested usage is supported</remarks>
    public class LogCastContextBranch : IDisposable
    {
        public int BranchId { get; }

        public static LogCastContextBranch Current => Context.Context.GetCurrent<LogCastContextBranch>();

        public LogCastContextBranch()
        {
            var context = LogCastContext.Current;
            if (context == null)
                throw new InvalidOperationException("LogCastContext is not started!");

            BranchId = context.GetNextBranchId();

            var parentBranch = Context.Context.Register(this);
            context.BranchHistory.Add(new LogCastBranchData(BranchId, parentBranch?.BranchId));
        }

        public void Dispose()
        {
            Context.Context.Unregister<LogCastContextBranch>();
        }
    }
}