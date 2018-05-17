using LogCast.Fallback;
using BddStyle.NUnit;
using Moq;

namespace LogCast.Test.given_FileFallbackLogger
{
    public abstract class Context : ContextBase
    {
        protected Mock<FileFallbackLogger> SutMock;
        protected Mock<IDeleteOldFiles> CleanupMock;
        protected const string BaseDir = "TestBaseDir";

        protected void InitSut(string dir)
        {
            CleanupMock = new Mock<IDeleteOldFiles>(MockBehavior.Loose);
            SutMock = new Mock<FileFallbackLogger>(MockBehavior.Strict, dir, CleanupMock.Object);
            SutMock.Setup(_ => _.AppendFallbackFile(It.IsAny<string>(), It.IsAny<string>()));
            SutMock.Setup(_ => _.GetTempPath()).Returns(BaseDir);
            SutMock.Setup(_ => _.EnsureDirectoryExists());
        }

        public override void Arrange()
        {
            InitSut("FallbackDir");
        }
    }
}