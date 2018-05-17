using System.IO;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_FileFallbackLogger
{
    public class when_writing_to_inaccessible_file : Context
    {
        public override void Arrange()
        {
            base.Arrange();
            SutMock.Setup(l => l.AppendFallbackFile(It.IsAny<string>(), It.IsAny<string>())).Throws<FileNotFoundException>();
        }

        [Test]
        public void then_with_constant_failure_only_three_attempts_made()
        {
            for (int i = 0; i < 300; i++)
                SutMock.Object.Write("failing_message");

            SutMock.Verify(l => l.AppendFallbackFile(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(100));
        }

        [Test]
        public void then_with_every_third_successful_message_all_attempts_are_made()
        {
            for (int i = 0; i < 100; i++)
            {
                SutMock.Setup(l => l.AppendFallbackFile(It.IsAny<string>(), It.IsAny<string>())).Throws<FileNotFoundException>();
                SutMock.Object.Write("failing_message");
                SutMock.Object.Write("failing_message");
                SutMock.Setup(l => l.AppendFallbackFile(It.IsAny<string>(), It.IsAny<string>()));
                SutMock.Object.Write("success_message");
            }

            SutMock.Verify(l => l.AppendFallbackFile(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(300));
        }
    }
}