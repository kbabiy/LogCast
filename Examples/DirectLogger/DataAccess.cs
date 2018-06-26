using System;
using LogCast;

namespace Examples.DirectLogger
{
    public class DataAccess : Call
    {
        public DataAccess(Random rnd, int maxDelay, int iterationCount) : base(rnd, maxDelay, iterationCount)
        {
        }

        protected override void LogDuration(int duration)
        {
            Log.AddContextProperty(Property.Durations.Name, "data", duration);
        }
    }
}