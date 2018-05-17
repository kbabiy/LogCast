using System.IO;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_DeleteOldFilesDaily.and_ran.and_days_to_keep_files_defaulted
{
    public class when_ran : Context
    {
        [Test]
        public void then_no_files_deleted()
        {
            SutMock.Verify(d => d.DeleteFile(It.IsAny<FileInfo>()), Times.Never);
        }
    }
}