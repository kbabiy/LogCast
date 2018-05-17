using System;

namespace LogCast.Utilities
{
    public class WinLoseDice
    {
        public int FailThreshold { get; }
        private readonly Random _random;

        public WinLoseDice(int failRate)
        {
            if (failRate <= 0)
            {
                FailThreshold = 0;
            }
            else if (failRate >= 100)
            {
                FailThreshold = 100;
            }
            else
            {
                FailThreshold = failRate;
                _random = new Random();
            }
        }

        /// <summary>
        /// Rolls the dice. Returns True when result is 'Win' and False otherwise
        /// </summary>
        public bool Roll()
        {
            return FailThreshold == 0
                   || FailThreshold != 100 && _random.Next(0, 100) > FailThreshold;
        }
    }
}