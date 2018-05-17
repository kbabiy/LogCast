using System;
using LogCast.Engine;
using LogCast.Rendering;
using BddStyle.NUnit;
using Moq;

namespace LogCast.Test.given_ContextSummaryBuilder
{
    public abstract class Context : ContextBase
    {
        internal ContextSummaryBuilder Builder;
        protected MessageFactory Factory;
        protected IContextTime Timer;
        internal ContextSummary Summary;

        public override void Arrange()
        {
            Timer = GetContextTimer(TimeSpan.FromHours(-1));
            Builder = new ContextSummaryBuilder(Timer, new DetailsFormatter());
            Factory = new MessageFactory("DefaultLogger", Timer.StartedAt);
        }

        public override void Act()
        {
            Summary = Builder.Build();
        }

        private static IContextTime GetContextTimer(TimeSpan startShift)
        {
            var now = DateTime.Now;
            var startTime = now.Add(startShift);
            var contextTimer = new Mock<IContextTime>(MockBehavior.Strict);
            contextTimer.Setup(t => t.StartedAt).Returns(startTime);
            contextTimer.Setup(t => t.Now).Returns(() => now);

            return contextTimer.Object;
        }

        protected LogMessage AddInfo(string message)
        {
            var logMessage = Factory.CreateMessage(LogLevel.Info, message);
            Builder.AddMessage(logMessage);
            return logMessage;
        }

        protected static string GetTrimmedMessge(char symbol, int count, int cutCount)
        {
            return $"{new string(symbol, count)} ...(cut {cutCount})";
        }
    }
}