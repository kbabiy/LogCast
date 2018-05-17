using LogCast.Utilities;
using BddStyle.NUnit;

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
    }
}