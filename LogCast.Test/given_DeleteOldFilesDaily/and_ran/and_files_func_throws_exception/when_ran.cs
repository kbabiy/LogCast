using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DeleteOldFilesDaily.and_ran.and_files_func_throws_exception
{
    public class when_ran : Context
    {
        private Exception _testException;
        protected override Func<IEnumerable<FileInfo>> FilesFunc =>
            () => throw _testException;

        public override void Arrange()
        {
            base.Arrange();
            _testException = new Exception();
        }

        [Test]
        public void then_thrown_exception_is_returned_as_result()
        {
            ResultErrors.Should().BeEquivalentTo(_testException);
        }
    }
}