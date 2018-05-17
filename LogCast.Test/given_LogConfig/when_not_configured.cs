using System;
using System.Diagnostics.CodeAnalysis;
using LogCast.Data;
using BddStyle.NUnit;
using NUnit.Framework;

namespace LogCast.Test.given_LogConfig
{
    public class when_not_configured : Context
    {
        [Test]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public void then_get_logger_throws_no_exceptions()
        {
            LogManager.GetLogger();
            LogManager.GetLogger("test1");
            LogManager.GetLogger(typeof(when_not_configured));
            LogManager.GetLogger<ContextBase>();
        }

        [Test]
        public void then_logging_throws_exception()
        {
            var logger = LogManager.GetLogger();
            Assert.Throws<InvalidOperationException>(() => logger.Info("My message"));
        }

        [Test]
        public void then_logcast_context_throws_exceptions()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (new LogCastContext())
                { }
            });
        }

        [Test]
        public void then_logging_in_unit_test_throws_no_exceptions()
        {
            LogConfig.DisableAutoConfig = false;
            var logger = LogManager.GetLogger();
            logger.Info("My message");
        }

        [Test]
        public void then_logcast_context_logging_in_unit_test_throws_no_exceptions()
        {
            LogConfig.DisableAutoConfig = false;
            using (var context = new LogCastContext())
            {
                var logger = LogManager.GetLogger();
                logger.Info("My message");

                context.Properties.Add(new LogProperty<int>("de", 34));
            }
        }
    }
}