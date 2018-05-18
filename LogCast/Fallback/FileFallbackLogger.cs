using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace LogCast.Fallback
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class FileFallbackLogger : IFallbackLogger
    {
        private readonly IDeleteOldFiles _cleanup;
        private byte _failureCount;
        private readonly string _dir;
        private readonly object _syncRoot = new object();

        private const string FileNameDateFormat = "yyyy-MM-dd";
        public const string LogFileSuffix = "_fallback.log";

        public FileFallbackLogger(string fallbackDir, int daysToKeepLogs)
            : this(fallbackDir, new DeleteOldFilesDaily(GetLogFileDate, daysToKeepLogs))
        {
        }

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public FileFallbackLogger([CanBeNull] string fallbackDir, [CanBeNull] IDeleteOldFiles cleanup)
        {
            if (string.IsNullOrWhiteSpace(fallbackDir))
                return;

            _cleanup = cleanup;
            try
            {
                fallbackDir = Cleanup(fallbackDir);

                //removing filename if any
                if (Path.HasExtension(Path.GetFileName(fallbackDir)))
                    fallbackDir = Path.GetDirectoryName(fallbackDir);

                if (string.IsNullOrEmpty(fallbackDir))
                    return;

                if (!Path.IsPathRooted(fallbackDir))
                {
                    var baseDir = GetTempPath();
                    fallbackDir = Path.Combine(baseDir, fallbackDir);
                }

                _dir = fallbackDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            catch
            {
                _dir = null;
            }
        }

        public void Write(params string[] messages)
        {
            WriteInternal(messages);
        }

        public void Write(Exception exception, params string[] messages)
        {
            WriteInternal(messages.Concat(new[] {exception.ToString()}));
        }

        private void WriteInternal(IEnumerable<string> messages)
        {
            if (_dir == null)
                return;

            if (_failureCount >= 100)
                return;

            try
            {
                var message = string.Join(Environment.NewLine, messages);
                message = $"{DateTime.Now:HH:mm:ss.ffff} | ERROR | {message}";

                EnsureDirectoryExists();

                var fileName = GetFileName(_dir, DateTime.Now);

                lock (_syncRoot)
                    AppendFallbackFile(message, fileName);

                var errors = _cleanup?.Run(ListLogs);

                if (errors != null && errors.Length > 0)
                {
                    var errorMessage = string.Join(Environment.NewLine,
                        errors.Select(e => $"[Cleanup]: {e.ToString()}"));
                    AppendFallbackFile(errorMessage, fileName);
                }

                _failureCount = 0;
            }
            catch
            {
                ++_failureCount;
            }
        }

        private static string Cleanup(string input)
        {
            input = Environment.ExpandEnvironmentVariables(input);
            var invalid = new HashSet<char>(Path.GetInvalidPathChars())
            {
                '%'//to flatten non-resolved environment variables
            };

            var filtered = input
                .Where(c => !invalid.Contains(c))
                .ToArray();

            var result = new string(filtered);
            return result;
        }

        protected internal virtual void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_dir))
                Directory.CreateDirectory(_dir);
        }

        private string _currentFileName;
        private StreamWriter _writer;

        protected internal virtual string GetTempPath()
        {
            return Path.Combine(Path.GetTempPath(), "LogCastTemp");
        }

        protected internal virtual void AppendFallbackFile(string message, string fileName)
        {
            if (_currentFileName != fileName)
            {
                _currentFileName = fileName;
                _writer?.Dispose();
                _writer = new StreamWriter(fileName, true);
            }

            _writer.WriteLine(message);
        }

        public bool Flush(TimeSpan timeout)
        {
            return _writer != null
                   && timeout.TotalMilliseconds > 0
                   && _writer.FlushAsync().Wait(timeout);
        }

        private IEnumerable<FileInfo> ListLogs()
        {
            return Directory.EnumerateFiles(_dir, $"*{LogFileSuffix}")
                .Select(fn => new FileInfo(fn));
        }

        internal static string GetFileName(string directory, DateTime date)
        {
            return Path.Combine(directory,
                $"{date.ToString(FileNameDateFormat, CultureInfo.InvariantCulture)}{LogFileSuffix}");
        }

        internal static DateTime? GetLogFileDate(FileInfo file)
        {
            var name = file.Name;
            var dateString = name.Substring(0, name.Length - LogFileSuffix.Length);

            return DateTime.TryParseExact(dateString, FileNameDateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var result)
                ? result
                : (DateTime?) null;
        }
    }
}