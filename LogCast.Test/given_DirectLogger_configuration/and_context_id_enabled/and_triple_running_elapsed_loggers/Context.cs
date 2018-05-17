using System;
using System.Collections.Generic;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled.and_triple_running_elapsed_loggers
{
    public abstract class Context : and_context_id_enabled.Context
    {
        private readonly TimeSpan _operationDelay = TimeSpan.FromMilliseconds(100);

        public override void Act()
        {
            RunElapsedLoggers(_operationDelay);
            RunElapsedLoggers(_operationDelay);
            RunElapsedLoggers(_operationDelay);
            Complete();
        }

        protected abstract void RunElapsedLoggers(TimeSpan delay);

        protected Dictionary<string, object> GetDurations()
        {
            return LastLog.GetProperty<Dictionary<string, object>>(Property.Durations.Name);
        }
    }
}