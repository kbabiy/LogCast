namespace LogCast.Test.given_DeleteOldFilesDaily.and_ran.and_days_to_keep_files_defaulted
{
    public abstract class Context : and_ran.Context
    {
        protected override int DaysToKeepFiles => 0;
    }
}