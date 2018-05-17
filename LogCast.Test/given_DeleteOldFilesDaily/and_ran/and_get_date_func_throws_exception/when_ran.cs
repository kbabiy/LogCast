using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DeleteOldFilesDaily.and_ran.and_get_date_func_throws_exception
{
    public class when_ran : Context
    {
        private Exception _testException;
        protected override Func<FileInfo, DateTime?> GetDateFunc =>
            _ => throw _testException;

        public override void Arrange()
        {
            base.Arrange();
            _testException = new Exception();
        }

        [Test]
        public void then_exception_for_each_file_is_returned_in_result()
        {
            var expected = Enumerable.Range(0, FileCount).Select(_ => _testException);
            ResultErrors.Should().BeEquivalentTo(expected);
        }
    }
}