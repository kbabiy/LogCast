using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_FileFallbackLogger.and_writing
{
    public class when_ran : Context
    {
        [Test]
        public void then_expected_message_written()
        {
            SutMock.Verify(f => f.AppendFallbackFile(
                    It.Is<string>(msg => msg.EndsWith("| ERROR | message")),
                    It.IsAny<string>()),
                Times.Once());
        }

        [Test]
        public void then_directory_is_ensured_to_exist()
        {
            SutMock.Verify(f => f.EnsureDirectoryExists(), Times.Once);
        }

        [Test]
        public void then_cleanup_is_called_with_expected_date()
        {
            CleanupMock.Verify(m => m.Run(
                It.IsAny<Func<IEnumerable<FileInfo>>>()),
                Times.Once);
        }
    }
}