using System;

namespace LogCast
{
    public interface IContextTime
    {
        DateTime StartedAt { get; }
        DateTime Now { get; }
    }
}