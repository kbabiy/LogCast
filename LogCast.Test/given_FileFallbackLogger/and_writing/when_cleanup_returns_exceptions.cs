using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_FileFallbackLogger.and_writing
{
    public class when_cleanup_returns_exceptions : Context
    {
        private Exception[] _testExceptions;

        public override void Arrange()
        {
            base.Arrange();

            _testExceptions = new[] { new Exception("exception1"), new Exception("exception2") };
            CleanupMock.Setup(c => c.Run(It.IsAny<Func<IEnumerable<FileInfo>>>())).Returns(_testExceptions);
        }

        [Test]
        public void then_exceptions_are_written_to_file()
        {
            SutMock.Verify(logger => logger.AppendFallbackFile(
                It.Is<string>(msg => msg.Contains("exception1") && msg.Contains("exception2")),
                It.IsAny<string>()),
            Times.Once());
        }
    }
}