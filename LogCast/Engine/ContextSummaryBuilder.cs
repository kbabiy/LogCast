using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogCast.Context;
using LogCast.Rendering;
using JetBrains.Annotations;

namespace LogCast.Engine
{
    internal class ContextSummaryBuilder
    {
        private const int MessageLengthLimit = 300;
        private const int LargeDetailsThreshold = 1 << 19; // 0.5 Mb

        private readonly BranchNode _rootNode;
        private readonly Dictionary<int, BranchNode> _allNodes;
        private readonly IContextTime _contextTime;
        private readonly IDetailsFormatter _formatter;
        private readonly bool _hasBranches;
        private const int RootBranchId = -1;

        public ContextSummaryBuilder([NotNull] IContextTime contextTime, [NotNull] IDetailsFormatter formatter)
        {
            _contextTime = contextTime ?? throw new ArgumentNullException(nameof(contextTime));
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _rootNode = new BranchNode(RootBranchId, null);
        }

        public ContextSummaryBuilder([NotNull] IContextTime contextTime, [NotNull] IDetailsFormatter formatter,
            [CanBeNull] IEnumerable<LogCastBranchData> branches)
            : this(contextTime, formatter)
        {
            _allNodes = BuildNodes(branches, _rootNode);
            _hasBranches = _allNodes?.Count > 0;
        }

        public void AddMessage(LogMessage message)
        {
            if (message.BranchId != null
                && _allNodes != null
                && _allNodes.TryGetValue(message.BranchId.Value, out var node))
            {
                node.RegisterMessage(message);
            }
            else
            {
                _rootNode.RegisterMessage(message);
            }
        }

        public ContextSummary Build()
        {
            if (_rootNode.Messages.Count == 0)
                return null;

            var info = new BuildInfo(_formatter.GetDetailsHeader());

            var contextStart = _contextTime.StartedAt;
            ProcessNode(_rootNode, null, contextStart, 0, info);

            var now = _contextTime.Now;
            var sinceLastMessage = now.Subtract(info.LastMessage.CreatedAt);
            info.Durations.Add((int) sinceLastMessage.TotalMilliseconds);
            info.DetailsBuilder.Append(_formatter.GetDetailsFooter(sinceLastMessage));

            var result = new ContextSummary
            {
                Level = info.MaxLevel,
                Details = info.DetailsBuilder.ToString(),
                Exceptions = info.Exceptions,
                Durations = info.Durations,
                Loggers = info.Loggers
            };

            var last = info.LastMessage?.OriginalMessage;

            result.Message = Trim(last, MessageLengthLimit);

            return result;
        }

        private void ProcessNode(BranchNode node, string branchName, DateTime previousTime, int nodeLevel, BuildInfo info)
        {
            var b = info.DetailsBuilder;

            // Within each node sort messages by creation time
            // "Enumerable.OrderBy" performs a stable sort - i.e. if dates are equal then elements order is preserved
            foreach (var message in node.Messages.OrderBy(x => x.CreatedAt))
            {
                // Usually context has no branches, so avoiding unnessesary type casts
                if (_hasBranches)
                {
                    if (message is BranchNode childNode)
                    {
                        string childBranchName = childNode.GetBranchName();
                        var childNodeLevel = nodeLevel + 1;

                        b.Append(_formatter.GetBranchStartMessage(childBranchName, childNodeLevel));

                        ProcessNode(childNode, childBranchName, previousTime, childNodeLevel, info);

                        b.Append(_formatter.GetBranchEndMessage(childBranchName, childNodeLevel));

                        continue;
                    }
                }

                TimeSpan elapsedTime = message.CreatedAt.Subtract(previousTime);
                previousTime = message.CreatedAt;

                if (b.Length > 0)
                    b.AppendLine();

                //if detail are getting too big, we cut each details entry severely to avoid large output
                var rendered = message.RenderedMessage;
                if (b.Length + rendered.Length > LargeDetailsThreshold)
                    message.RenderedMessage = Trim(rendered, MessageLengthLimit);

                rendered = _formatter.FormatContextMessage(message, elapsedTime, branchName, nodeLevel);

                b.Append(rendered);

                info.LastMessage = message;
                info.Durations.Add((int) elapsedTime.TotalMilliseconds);

                info.Loggers.Add(message.LoggerName);

                if (info.MaxLevel < message.Level)
                    info.MaxLevel = message.Level;

                var exception = message.Exception;
                if (exception != null)
                    info.Exceptions.Add(exception);
            }
        }

        private static string Trim([CanBeNull] string last, int limit)
        {
            if (last == null)
                return last;

            var overLimit = last.Length - limit;

            if (overLimit <= 0)
                return last;

            return $"{last.Substring(0, limit)} ...(cut {overLimit})";
        }

        private static Dictionary<int, BranchNode> BuildNodes(IEnumerable<LogCastBranchData> branches, BranchNode rootNode)
        {
            if (branches == null)
                return null;

            // Build a dictionary with all node IDs for subsequent lookups
            Dictionary<int, BranchNode> allNodes = branches.ToDictionary(x => x.Id, x => new BranchNode(x.Id, x.ParentId));
            if (allNodes.Count == 0)
                return null;

            // Setup parent object for each node
            foreach (var node in allNodes.Values)
            {
                if (node.ParentId == null)
                {
                    node.Parent = rootNode;
                }
                else
                {
                    node.Parent = allNodes.TryGetValue(node.ParentId.Value, out var parentNode) ? parentNode : rootNode;
                }
            }

            return allNodes;
        }

        private class BuildInfo
        {
            public BuildInfo(string detailsHeader)
            {
                DetailsBuilder = new StringBuilder(detailsHeader);
                Durations = new List<int>();
                Loggers = new HashSet<string>(StringComparer.Ordinal);
                Exceptions = new List<Exception>();
            }

            public LogLevel MaxLevel;
            public LogMessage LastMessage;
            public readonly StringBuilder DetailsBuilder;

            public readonly List<Exception> Exceptions;
            public readonly List<int> Durations;
            public readonly HashSet<string> Loggers;
        }

        // Inherit from message to be able to sort both messages and nodes by 'CreatedAt'
        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private class BranchNode : LogMessage
        {
            private readonly int _id;

            public int? ParentId { get; }
            public BranchNode Parent { get; set; }
            public List<LogMessage> Messages { get; }

            public BranchNode(int id, int? parentId)
            {
                _id = id;
                ParentId = parentId;
                Messages = new List<LogMessage>();
                CreatedAt = DateTime.MinValue;
            }

            public void RegisterMessage(LogMessage message)
            {
                // As soon as the first message of the branch arrives, we register the branch as a "message" in the parent branch
                if (Messages.Count == 0)
                {
                    Parent?.RegisterMessage(this);
                }
                Messages.Add(message);
                CheckCreatedDate(message.CreatedAt);
            }

            public string GetBranchName()
            {
                return $"{(Parent == null || Parent._id == RootBranchId ? null : Parent.GetBranchName() + "-")}{_id}";
            }

            private void CheckCreatedDate(DateTime created)
            {
                if (CreatedAt == DateTime.MinValue || created < CreatedAt)
                {
                    // Consider node's creation date the date of its earliest message
                    CreatedAt = created;

                    // From the parent node perspective this node is also a message thus it affects the parent's creation date
                    Parent?.CheckCreatedDate(created);
                }
            }
        }
    }
}