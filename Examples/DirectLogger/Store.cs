using System;
using System.Linq;
using LogCast;

namespace Examples.DirectLogger
{
    public class Store : Call
    {
        public Store(Random rnd, int maxDelay, int iterationCount) : base(rnd, maxDelay, iterationCount)
        {
        }

        protected override void LogDuration(int duration)
        {
            Log.AddContextProperty(Property.Durations.Name, "storage-access-total",
                duration,
                i => i.Sum());
        }
    }
}