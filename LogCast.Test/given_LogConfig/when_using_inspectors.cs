using System.Diagnostics.CodeAnalysis;
using LogCast.Engine;
using LogCast.Inspectors;
using LogCast.Loggers;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_LogConfig
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class when_using_inspectors : Context
    {
        private Mock<ILogManager> _managerMock;
        private Inspectors _inspectors;

        public override void Arrange()
        {
            _managerMock = new Mock<ILogManager>();
            _inspectors = new Inspectors
            {
                Inspector1 = new Inspector1(),
                Inspector3 = new Inspector3(),
                Inspector4 = new Inspector4()
            };
            base.Arrange();
        }

        public override void Act()
        {
            LogConfig.BeginConfiguration(_managerMock.Object)
                .WithGlobalInspector(_inspectors.Inspector1)
                .WithGlobalInspector(_inspectors.Inspector3)
                .WithGlobalInspector(_inspectors.Inspector4)
                .End();
            base.Act();
        }

        [Test]
        public void then_specified_inspectors_are_registered()
        {
            var config = LogConfig.Current;
            config.Engine.GetInspector<Inspector1>().Should().NotBeNull();
            config.Engine.GetInspector<Inspector3>().Should().NotBeNull();
            config.Engine.GetInspector<Inspector4>().Should().NotBeNull();
        }

        [Test]
        public void then_inspectors_are_matched_by_exact_type()
        {
            var config = LogConfig.Current;
            config.Engine.GetInspector<Inspector0>().Should().BeNull();
            config.Engine.GetInspector<Inspector2>().Should().BeNull();
        }

        private class Inspectors
        {
            public Inspector1 Inspector1;
            public Inspector3 Inspector3;
            public Inspector4 Inspector4;
        }

        public class Inspector0 : ILogDispatchInspector
        {
            public void BeforeSend(LogCastDocument document, LogMessage sourceMessage)
            {
            }

            public void BeforeSend(LogCastDocument document, LogCastContext sourceContext)
            {
            }
        }

        public class Inspector1 : Inspector0
        {
        }

        public class Inspector2 : Inspector0
        {
        }

        public class Inspector3 : Inspector1
        {
        }

        public class Inspector4 : Inspector2
        {
        }
    }
}