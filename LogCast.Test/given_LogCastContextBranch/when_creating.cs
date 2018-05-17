using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastContextBranch
{
    public class when_creating : Context
    {
        private int? _branchId;
        private LogCastContextBranch _branchBefore;
        private LogCastContextBranch _branchAfter;
        public override void Act()
        {
            using (new LogCastContext())
            {
                _branchBefore = CurrentBranch;
                using (new LogCastContextBranch())
                {
                    _branchId = CurrentBranch?.BranchId;
                }
                _branchAfter = CurrentBranch;
            }
        }

        [Test]
        public void then_no_branch_out_of_using_scope()
        {
            _branchBefore.Should().BeNull();
            _branchAfter.Should().BeNull();
        }

        [Test]
        public void then_branch_id_should_be_1()
        {
            _branchId.Should().Be(1);
        }
    }
}