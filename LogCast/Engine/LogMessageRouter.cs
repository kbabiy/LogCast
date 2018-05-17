using LogCast.Utilities;
using JetBrains.Annotations;

namespace LogCast.Engine
{
    public class LogMessageRouter
    {
        private readonly WinLoseDice _logDice;

        public LogMessageRouter(int skipPercentage)
        {
            if (skipPercentage > 0)
            {
                _logDice = new WinLoseDice(skipPercentage);
            }
        }

        [UsedImplicitly]
        public bool DispatchMessage(LogMessage message)
        {
            var context = LogCastContext.Current;
            if (SkipMessage(context))
                return false;

            if (context == null)
            {
                LogConfig.Current.Engine.SendContextLessMessage(message);
            }
            else
            {
                message.BranchId = LogCastContextBranch.Current?.BranchId;
                bool isAdded = context.PendingMessages.Add(message);
                if (!isAdded)
                {
                    LogConfig.Current.Engine.SendContextLessMessage(message);
                }
            }

            return true;
        }

        private bool SkipMessage(LogCastContext context)
        {
            if (_logDice == null)
                return false;

            // When there's no context roll the dice per message
            if (context == null)
                return !_logDice.Roll();

            // When context is available roll the dice once per context
            if (context.SuppressMessages == null)
            {
                context.SuppressMessages = !_logDice.Roll();
            }
            return context.SuppressMessages == true;
        }
    }
}
