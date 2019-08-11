using System;
using System.Linq;
using System.Threading.Tasks;
using LogCast.Utilities;
using BddStyle.NUnit;
using FluentAssertions;

namespace LogCast.Test.given_CountEvent
{
    public abstract class Context : ContextBase
    {
        protected const int ThreadCount = 100;
        protected CountEvent Sut;
        public override void Arrange()
        {
            Sut = new CountEvent();
        }
        
        protected Task[] StartIncreases()
        {
            return Enumerable.Range(0, ThreadCount).Select(
                i => Task.Factory.StartNew(Sut.Increase)).ToArray();
        }       
        
        protected Task[] StartDecreases()
        {
            return Enumerable.Range(0, ThreadCount).Select(
                i => Task.Factory.StartNew(Sut.Decrease)).ToArray();
        }

        protected T Timed<T>(TimeSpan frame, Func<T> action)
        {
            T result = default(T);
            var inTime = Task.Factory.StartNew(() =>
            {
                return result = action();
            }).Wait(frame);

            inTime.Should().BeTrue("Operation is expected to finish within the time frame");
            
            return result;
        }
    }
}