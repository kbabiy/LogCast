using System;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_FileFallbackLogger
{
    public class when_writing_with_exception : Context
    {
        class TestException : Exception
        {
            public override string ToString()
            {
                return "exception";
            }
        }

        public override void Act()
        {
            SutMock.Object.Write(new TestException(), "message");
        }

        [Test]
        public void then_expected_message_written()
        {
            string expectedMessage = $@"| ERROR | message{Environment.NewLine}exception";

            SutMock.Verify(logger => logger.AppendFallbackFile(It.Is<string>(msg => msg.EndsWith(expectedMessage)),
                    It.IsAny<string>()),
                Times.Once());
        }
    }
}