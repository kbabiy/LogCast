using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DeleteOldFilesDaily.and_ran.and_no_files_returned
{
    public class when_ran_twice : Context
    {
        public override void Act()
        {
            base.Act();
            base.Act();
        }

        [Test]
        public void then_error_is_empty()
        {
            ResultErrors.Should().BeNull();
        }
    }
}