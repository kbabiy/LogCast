using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastContextBranch
{
    public class when_creating_nested_branches : Context
    {
        private LogCastContextBranch _branchBefore;
        private LogCastContextBranch _branchAfter;
        private int? _branchIdBefore1;
        private int? _branchIdAfter1;
        private int? _branchIdBefore2;
        private int? _branchIdAfter2;
        private int? _branchId3;

        public override void Act()
        {
            using (new LogCastContext())
            {
                _branchBefore = CurrentBranch;
                using (new LogCastContextBranch())
                {
                    _branchIdBefore1 = CurrentBranch?.BranchId;
                    using (new LogCastContextBranch())
                    {
                        _branchIdBefore2 = CurrentBranch?.BranchId;
                        using (new LogCastContextBranch())
                        {
                            _branchId3 = CurrentBranch?.BranchId;
                        }
                        _branchIdAfter2 = CurrentBranch?.BranchId;
                    }
                    _branchIdAfter1 = CurrentBranch?.BranchId;
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
        public void then_root_branch_id_should_be_1()
        {
            _branchIdBefore1.Should().Be(1);
            _branchIdAfter1.Should().Be(1);
        }

        [Test]
        public void then_child_branch_id_should_be_2()
        {
            _branchIdBefore2.Should().Be(2);
            _branchIdAfter2.Should().Be(2);
        }

        [Test]
        public void then_sub_child_branch_id_should_be_3()
        {
            _branchId3.Should().Be(3);
        }
    }
}