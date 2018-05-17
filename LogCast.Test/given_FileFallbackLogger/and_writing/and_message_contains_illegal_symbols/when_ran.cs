using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_FileFallbackLogger.and_writing.and_message_contains_illegal_symbols
{
    public class when_ran : Context
    {
        protected override bool SuppressAct => true;

        [TestCase("{0}")]
        [TestCase("{1}message{0}")]
        [TestCase("//\\\\")]
        public void then_proper_error_logged(string message)
        {
            Write(message);

            SutMock.Verify(f => f.AppendFallbackFile(
                    It.Is<string>(msg => msg.EndsWith("| ERROR | " + message)),
                    It.IsAny<string>()),
                Times.Once());
        }
    }
}