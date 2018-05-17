using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DeleteOldFilesDaily.and_ran.and_no_files_returned
{
    public class when_ran : Context
    {
        [Test]
        public void then_no_errors()
        {
            ResultErrors.Should().BeNull();
        }
    }
}