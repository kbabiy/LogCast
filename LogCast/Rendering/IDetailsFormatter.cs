using System;
using JetBrains.Annotations;

namespace LogCast.Rendering
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public interface IDetailsFormatter
    {
        string FormatStandaloneMessage(LogMessage message);
        string FormatContextMessage(LogMessage message, TimeSpan timeElapsed, string branchName, int branchLevel);

        // Called in ContextMode only
        string GetDetailsHeader();
        string GetDetailsFooter(TimeSpan timeSinceLastMessage);
        string GetBranchStartMessage(string branchName, int branchLevel);
        string GetBranchEndMessage(string branchName, int branchLevel);
    }
}