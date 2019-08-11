using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;

namespace LogCast.Utilities
{
    /// <summary>
    /// Registers Increase and Decrease call counts and allows waiting the count to reach 0 
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class CountEvent
    {
        private readonly object _counterChangeLock = new object();
        private int _counter;

        public int Count => Thread.VolatileRead(ref _counter);

        public void Increase()
        {
            lock (_counterChangeLock)
            {
                _counter++;
                Monitor.Pulse(_counterChangeLock);
            }
        }

        public void Decrease()
        {
            lock (_counterChangeLock)
            {
                _counter--;
                Monitor.Pulse(_counterChangeLock);
            }
        }

        public bool WaitUntil(int targetValue, TimeSpan howLong)
        {
            var checkInterval = howLong <= TimeSpan.Zero ? TimeSpan.Zero
                : new TimeSpan(howLong.Ticks / 16);

            var sw = Stopwatch.StartNew();
            lock (_counterChangeLock)
            {
                while (_counter > targetValue && sw.Elapsed < howLong)
                {
                    Monitor.Wait(_counterChangeLock, checkInterval);
                }

                return _counter <= targetValue;
            }
        }
    }
}