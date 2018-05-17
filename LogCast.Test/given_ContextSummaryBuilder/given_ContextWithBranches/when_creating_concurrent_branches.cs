using System;
using System.Collections.Generic;
using System.Linq;
using LogCast.Context;
using LogCast.Engine;
using LogCast.Rendering;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_ContextSummaryBuilder.given_ContextWithBranches
{
    public class when_creating_concurrent_branches : Context
    {
        private const int MessagesPerBranch = 10;
        private const int RootBranchesCount = 2;
        private const int ChildBranchesPerRoot = 5;

        private List<int> _childBranches;

        
        private const string ContextLogger = "ContextLogger";
        private const string BranchLogger = "BranchLogger";

        public override void Arrange()
        {
            base.Arrange();
            // Generate branch structure
            // Root branches
            var branches = new List<LogCastBranchData>();
            for (int j = 0; j < RootBranchesCount; j++)
            {
                branches.Add(new LogCastBranchData(j, null));
            }

            // Child branches
            _childBranches = new List<int>();
            int childBranchId = RootBranchesCount + 100;
            for (int k = 0; k < ChildBranchesPerRoot; k++)
            {
                _childBranches.Add(childBranchId);
                branches.Add(new LogCastBranchData(childBranchId, k));
                childBranchId++;
            }

            Builder = new ContextSummaryBuilder(Timer, new DetailsFormatter(), branches);
            Factory = new MessageFactory(ContextLogger, Timer.StartedAt) {MessageIntervalMs = 1};
        }

        public override void Act()
        {
            // Generate messages
            for (int i = 0; i < MessagesPerBranch; i++)
            {
                Builder.AddMessage(Factory.CreateMessage(LogLevel.Warn, $"context {i} message"));
                for (int j = 0; j < RootBranchesCount; j++)
                {
                    var logger = BranchLogger + j;
                    var message = Factory.CreateMessage(logger, LogLevel.Info, $"root branch {j} message");
                    message.BranchId = j;
                    Builder.AddMessage(message);

                    foreach (var childId in _childBranches)
                    {
                        message = Factory.CreateMessage(logger, LogLevel.Error, $"child branch {childId} message",
                            new ArgumentNullException());
                        message.BranchId = childId;
                        Builder.AddMessage(message);
                    }
                }
            }

            base.Act();
        }

        [Test]
        public void then_all_loggers_present_in_summary()
        {
            var expectedLoggers = new List<string> {ContextLogger};
            for (int i = 0; i < RootBranchesCount; i++)
                expectedLoggers.Add(BranchLogger + i);

            Summary.Loggers.Should().BeEquivalentTo(expectedLoggers);
        }

        [Test]
        public void then_log_level_is_max()
        {
            Summary.Level.Should().Be(LogLevel.Error);
        }

        [Test]
        public void then_message_is_the_last_message()
        {
            // The first message is rendered "normally" but then branches occupy the space and only after all branches 
            // have been completed "free" messages continue to render.
            Summary.Message.Should().Be($"context {MessagesPerBranch - 1} message");
        }

        [Test]
        public void then_exception_is_present()
        {
            var expectedCount = MessagesPerBranch * RootBranchesCount * ChildBranchesPerRoot;
            var expectedExceptions = Enumerable.Range(0, expectedCount).Select(i => "ArgumentNullException");

            Summary.Exceptions.Select(e => e.GetType().Name).Should().BeEquivalentTo(expectedExceptions);
        }
    }
}