using System;
using System.Linq;
using System.Threading.Tasks;
using LogCast;

namespace Examples.DirectLogger
{
    // with/without context
    // errors
    // slowdowns
    // certain calls
    // whatsoever

    public class Scenarios
    {
        private static readonly Random Rnd = new Random();
        private readonly Call[] _calls;

        public Scenarios()
        {
            _calls = new Call[]
            {
                new DataAccess(Rnd, 100, 3),
                new Compute(Rnd, 10, 5),
                new Store(Rnd, 200, 2),
                new SatisfyCustomerNeeds(Rnd, 50, 3)
            };
        }

        private Call NextCall()
        {
            var i = Rnd.Next(_calls.Length);
            return _calls[i];
        }

        public void Start()
        {
            var tasks = Enumerable.Range(0, 10)
                .Select(async i => await Cycle())
                .ToArray();

            Task.WaitAll(tasks);
        }

        private async Task Cycle()
        {
            while (true)
            {
                using (new LogCastContext())
                {
                    var next = NextCall();
                    await next.Run();
                }
            }
        }
    }
}