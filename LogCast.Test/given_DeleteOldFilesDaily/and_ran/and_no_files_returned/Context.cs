using System;
using System.Collections.Generic;
using System.IO;

namespace LogCast.Test.given_DeleteOldFilesDaily.and_ran.and_no_files_returned
{
    public abstract class Context : and_ran.Context
    {
        protected override Func<IEnumerable<FileInfo>> FilesFunc => () => null;
    }
}