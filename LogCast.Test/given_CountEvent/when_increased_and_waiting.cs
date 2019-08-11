using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent
{
    public class when_increased_and_waiting : Context
    {
        public override void Act()
        {
            Sut.Increase();
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