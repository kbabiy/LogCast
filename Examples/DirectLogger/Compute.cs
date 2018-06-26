using System;

namespace Examples.DirectLogger
{
    public class Compute : Call
    {
        public Compute(Random rnd, int maxDelay, int iterationCount) : base(rnd, maxDelay, iterationCount)
        {
        }

        protected override void LogError()
        {
            Log.Error(new Exception("General computation issues"));
        }
    }
}