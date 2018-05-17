using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogCast.Fallback;
using BddStyle.NUnit;
using Moq;

namespace LogCast.Test.given_DeleteOldFilesDaily
{
    public abstract class Context : ContextBase
    {
        protected virtual Func<IEnumerable<FileInfo>> FilesFunc =>
            () => new[] { new FileInfo("1"), new FileInfo("0"), new FileInfo("-1"), OldFile1, OldFile2 };

        protected int FileCount => FilesFunc().Count();

        protected virtual Func<FileInfo, DateTime?> GetDateFunc =>
            f =>
            {
                int daysShift = int.Parse(f.Name);
                return Today.AddDays(daysShift);
            };

        protected virtual int DaysToKeepFiles => 1;

        protected Mock<DeleteOldFilesDaily> SutMock;
        protected FileInfo OldFile1;
        protected FileInfo OldFile2;
        protected Exception[] ResultErrors;
        protected DateTime Today;

        public override void Arrange()
        {
            Today = DateTime.Today;
            OldFile1 = new FileInfo("-2");
            OldFile2 = new FileInfo("-3");
            SutMock = new Mock<DeleteOldFilesDaily>(MockBehavior.Strict, GetDateFunc, DaysToKeepFiles);
            SutMock.Setup(del => del.DeleteFile(It.IsAny<FileInfo>()));
        }
    }
}