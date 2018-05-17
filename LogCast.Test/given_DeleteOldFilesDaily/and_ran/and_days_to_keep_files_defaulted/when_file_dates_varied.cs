using System;
using System.IO;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_DeleteOldFilesDaily.and_ran.and_days_to_keep_files_defaulted
{
    public class when_file_dates_varied : Context
    {
        protected override bool SuppressAct => true;

        private int _shiftBackFromToday;
        protected override Func<FileInfo, DateTime?> GetDateFunc =>
            _ => Today.Subtract(TimeSpan.FromDays(_shiftBackFromToday));

        [TestCase(1, false)]
        [TestCase(6, false)]
        [TestCase(7, false)]
        [TestCase(8, true)]
        [TestCase(100, true)]
        public void then_delete_call_happens_as_expected(int shiftBackFromToday, bool deleteCalled)
        {
            _shiftBackFromToday = shiftBackFromToday;
            Act();

            var times = deleteCalled ? Times.Exactly(FileCount) : Times.Never();

            SutMock.Verify(d => d.DeleteFile(It.IsAny<FileInfo>()), times);
        }
    }
}