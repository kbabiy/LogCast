using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled.and_emtpy_context_processing_suppressed
{
    public class when_completing_with_branch_set : Context
    {
        private LogCastContextBranch _branch;

        public override void Arrange()
        {
            base.Arrange();
            _branch = new LogCastContextBranch();
        }

        public override void Cleanup()
        {
            _branch.Dispose();
            base.Cleanup();
        }

        [Test]
        public void then_no_log_message_created()
        {
            LastLog.Should().BeNull();
        }
    }
}