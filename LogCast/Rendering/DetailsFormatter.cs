using System;
using System.Diagnostics.CodeAnalysis;

namespace LogCast.Rendering
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class DetailsFormatter : IDetailsFormatter
    {
        private const string DurationPattern = "({0}) ----------";
        private static readonly string MessageHeaderPattern = DurationPattern + Environment.NewLine;
        private static readonly string BranchMessageHeaderPattern = "{1}" + DurationPattern + Environment.NewLine + "{1}";
        private static readonly string DetailsFooterPattern = Environment.NewLine + DurationPattern;

        public virtual string GetDetailsHeader() => null;

        public virtual string GetDetailsFooter(TimeSpan timeSinceLastMessage)
        {
            return string.Format(DetailsFooterPattern, (int) timeSinceLastMessage.TotalMilliseconds);
        }

        public virtual string GetBranchStartMessage(string branchName, int branchLevel)
        {
            return $"{Environment.NewLine}{GetIndent(branchLevel)}[B{branchName}] start";
        }

        public virtual string GetBranchEndMessage(string branchName, int branchLevel)
        {
            return $"{Environment.NewLine}{GetIndent(branchLevel)}[B{branchName}] end";
        }

        public virtual string FormatStandaloneMessage(LogMessage message)
        {
            return Format(message);
        }

        public virtual string FormatContextMessage(LogMessage message, TimeSpan timeElapsed, string branchName, int branchLevel)
        {
            var header = string.IsNullOrEmpty(branchName)
                ? string.Format(MessageHeaderPattern, (int) timeElapsed.TotalMilliseconds)
                : string.Format(BranchMessageHeaderPattern, (int) timeElapsed.TotalMilliseconds, GetIndent(branchLevel + 1));

            return header + Format(message);
        }

        private static string Format(LogMessage message)
        {
            return message.RenderedMessage
                   + (message.Exception == null
                       ? null
                       : Environment.NewLine + message.Exception);
        }

        private static string GetIndent(int level)
        {
            return new string(' ', 4 * level);
        }
    }
}