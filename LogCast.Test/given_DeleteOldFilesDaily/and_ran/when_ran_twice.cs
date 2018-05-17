using System.IO;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_DeleteOldFilesDaily.and_ran
{
    public class when_ran_twice : Context
    {
        public override void Act()
        {
            base.Act();
            base.Act();
        }

        [Test]
        public void then_delete_is_called_2_times_not_4()
        {
            SutMock.Verify(del => del.DeleteFile(It.IsAny<FileInfo>()), Times.Exactly(2));
        }
    }
}