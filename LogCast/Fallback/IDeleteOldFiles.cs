using System;
using System.Collections.Generic;
using System.IO;

namespace LogCast.Fallback
{
    public interface IDeleteOldFiles
    {
        Exception[] Run(Func<IEnumerable<FileInfo>> files);
    }
}