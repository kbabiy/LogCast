using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;

namespace LogCast.Utilities
{
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
            // When TimeSpan.MaxValue is specified then TotalMilliseconds will be more than int.MaxValue
            var checkInterval = howLong.TotalMilliseconds <= 0 
                ? 0 
                : (howLong.TotalMilliseconds > int.MaxValue ? int.MaxValue : (int)howLong.TotalMilliseconds / 20);
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