namespace LogCast.Test.given_DeleteOldFilesDaily.and_ran
{
    public abstract class Context : given_DeleteOldFilesDaily.Context
    {
        public override void Act()
        {
            ResultErrors = SutMock.Object.Run(FilesFunc);
        }
    }
}