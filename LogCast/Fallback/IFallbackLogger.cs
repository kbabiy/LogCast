using System;
using JetBrains.Annotations;

namespace LogCast.Fallback
{
    public interface IFallbackLogger
    {
        void Write([NotNull] params string[] messages);
        void Write([NotNull] Exception exception, [NotNull] params string[] messages);
        bool Flush(TimeSpan timeout);
    }
}