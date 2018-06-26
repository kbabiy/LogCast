using System;
using System.Threading.Tasks;
using LogCast;

namespace Examples.DirectLogger
{
    public abstract class Call
    {
        protected readonly ILogger Log;
        private readonly Random _rnd;
        private readonly int _delay;
        private readonly int _iterationCount;

        protected Call(Random rnd, int maxDelay, int iterationCount)
        {
            Log = LogManager.GetLogger(GetType());
            _delay = rnd.Next(maxDelay);
            _iterationCount = iterationCount;
            _rnd = rnd;
        }

        private bool Slowdown => _rnd.Next(100) > 90;
        private bool Error => _rnd.Next(100) > 96;

        public async Task Run()
        {
            for (int i = 0; i < _iterationCount; i++)
            {
                Log.Info($"Starting operation {i}");
                await Task.Delay(_delay);
                LogDuration(_delay);
                if (Slowdown)
                    await Task.Delay(_delay * 10);
                if (Error)
                    LogError();

                Log.Info("Finished");
            }
        }

        protected virtual void LogDuration(int duration)
        {
        }

        protected virtual void LogError()
        {
            Log.Error("General issue happenned, whoa?");
        }
    }
}