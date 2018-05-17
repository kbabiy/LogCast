using LogCast.Loggers.Elapsed;
using BddStyle.NUnit;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DurationsSumElapsedLogger.and_context_does_not_exist
{
    public class when_disposing : ContextBase
    {
        private DurationsSumElapsedLogger _sut;

        public override void Arrange()
        {
            base.Arrange();

            LogCastContext.Current?.Dispose();
            _sut = new DurationsSumElapsedLogger("any");
        }

        [Test]
        public void then_dispose_doesnt_throw_exceptions()
        {
            _sut.Invoking(_ => _.Dispose()).Should().NotThrow();
        }
    }
}