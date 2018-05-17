using LogCast.Loggers.Elapsed;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DurationsSumElapsedLogger.and_context_exisits.and_logger_was_disposed
{
    public class when_disposing : Context
    {
        private DurationsSumElapsedLogger _sut;

        public override void Arrange()
        {
            base.Arrange();

            _sut = new DurationsSumElapsedLogger(OperationName);
            _sut.Dispose();
        }

        public override void Act()
        {
            _sut.Dispose();
        }

        [Test]
        public void then_new_value_is_not_added()
        {
            GetOperationDurationProperties(CurrentContext, OperationName)
                .Should().HaveCount(1);
        }
    }
}
