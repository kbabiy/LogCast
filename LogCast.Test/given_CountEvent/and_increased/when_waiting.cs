using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent.and_increased
{
    public class when_waiting : Context
    {
        public override void Act()
        {
            Task.Factory.StartNew(() => Sut.WaitUntil(0, TimeSpan.FromMinutes(1)));
            Thread.Sleep(100);
        }
        
        [Test]
        public void then_next_decrease_is_not_blocked()
        {
            Sut.Decrease();
        }

        [Test]
        public void then_next_increase_is_not_blocked()
        {
            Sut.Increase();
        }
    }
}