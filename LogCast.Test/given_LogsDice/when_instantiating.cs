using LogCast.Utilities;
using NUnit.Framework;
using BddStyle.NUnit;

namespace LogCast.Test.given_LogsDice
{
    public class when_instantiating : ContextBase
    {
        [TestCase(-11, ExpectedResult = 0)]
        [TestCase(0, ExpectedResult = 0)]
        [TestCase(42, ExpectedResult = 42)]
        [TestCase(100, ExpectedResult = 100)]
        [TestCase(1111, ExpectedResult = 100)]
        public int then_fail_rate_boundary_is_set_correctly(int failRate)
        {
            var sut = new WinLoseDice(failRate);
            return sut.FailThreshold;
        }
    }
}