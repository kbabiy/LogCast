using System.Threading.Tasks;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled.and_context_returned_from_long_running_thread
{
    public abstract class Context : and_context_id_enabled.Context
    {
        private Task<LogCastContext> _task;
        protected LogCastContext InnerContext;

        public override void Arrange()
        {
            base.Arrange();
            _task = new Task<LogCastContext>(() => LogCastContext.Current);
            Complete();
        }

        public override void Act()
        {
            base.Act();
            _task.Start();
            InnerContext = _task.Result;
        }
    }
}