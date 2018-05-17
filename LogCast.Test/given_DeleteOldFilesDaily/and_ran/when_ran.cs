using System.IO;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_DeleteOldFilesDaily.and_ran
{
    public class when_ran : Context
    {
        [Test]
        public void then_2_files_deleted()
        {
            SutMock.Verify(del => del.DeleteFile(It.IsAny<FileInfo>()), Times.Exactly(2));
        }

        [Test]
        public void then_first_old_file_deleted()
        {
            SutMock.Verify(del => del.DeleteFile(OldFile1), Times.Once);
        }

        [Test]
        public void then_second_old_file_deleted()
        {
            SutMock.Verify(del => del.DeleteFile(OldFile2), Times.Once);
        }

        [Test]
        public void then_no_errors()
        {
            ResultErrors.Should().BeNull();
        }
    }
}