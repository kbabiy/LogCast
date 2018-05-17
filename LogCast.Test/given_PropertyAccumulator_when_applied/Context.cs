using LogCast.Engine;
using BddStyle.NUnit;

namespace LogCast.Test.given_PropertyAccumulator_when_applied
{
    public abstract class Context : ContextBase
    {
        internal PropertyAccumulator Accumulator;
        protected LogCastDocument Document;
         
        public override void Arrange()
        {
            Accumulator = new PropertyAccumulator();
            Document = new LogCastDocument();
        }

        public override void Act()
        {
            Accumulator.Apply(Document);
        }
    }
}
