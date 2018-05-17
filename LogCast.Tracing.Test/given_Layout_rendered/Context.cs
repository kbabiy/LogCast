using System;
using LogCast.Rendering;
using BddStyle.NUnit;

namespace LogCast.Tracing.Test.given_Layout_rendered
{
    public abstract class Context : ContextBase
    {
        protected const string TestPattern = "{date:ddd, hh-mm-ss} | {logger} | {level} | {message}";
        internal MessageLayout Sut;
        protected string ParseResult;

        public override void Arrange()
        {
            Sut = new MessageLayout(TestPattern);
        }

        public override void Act()
        {
            ParseResult = Sut.Render("Test message", "LoggerName", LogLevel.Info, new DateTime(2012, 12, 1, 4, 3, 2));
        }
    }
}