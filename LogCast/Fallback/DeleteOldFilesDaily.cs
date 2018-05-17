using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace LogCast.Fallback
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class DeleteOldFilesDaily : IDeleteOldFiles
    {
        private readonly Func<FileInfo, DateTime?> _getFileDate;
        private readonly int _daysToKeepFiles;
        private DateTime? _lastRan;
        private readonly object _syncRoot = new object();

        public DeleteOldFilesDaily([NotNull] Func<FileInfo, DateTime?> getFileDate, int daysToKeepFiles)
        {
            _getFileDate = getFileDate ?? throw new ArgumentNullException(nameof(getFileDate));
            _daysToKeepFiles = daysToKeepFiles > 0 ? daysToKeepFiles : 7;
        }

        [CanBeNull]
        public Exception[] Run([NotNull] Func<IEnumerable<FileInfo>> files)
        {
            try
            {
                var today = DateTime.Today;

                if (_lastRan == today)
                    return null;

                lock (_syncRoot)
                {
                    if (_lastRan == today)
                        return null;

                    _lastRan = today;
                }

                var errors = files()?
                    .Select(CheckAndDelete)
                    .Where(ex => ex != null)
                    .ToArray();

                if (errors?.Length == 0)
                    errors = null;

                return errors;
            }
            catch (Exception e)
            {
                return new[] {e};
            }
        }

        private Exception CheckAndDelete(FileInfo file)
        {
            try
            {
                var lastDayToKeepFiles = DateTime.Now.Subtract(TimeSpan.FromDays(_daysToKeepFiles)).Date;
                if (_getFileDate(file) < lastDayToKeepFiles)
                    DeleteFile(file);
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }

        protected internal virtual void DeleteFile(FileInfo file)
        {
            file.Delete();
        }
    }
}