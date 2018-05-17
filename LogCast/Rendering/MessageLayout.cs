using System;
using System.Globalization;
using System.Text;
using System.Threading;

namespace LogCast.Rendering
{
    public class MessageLayout
    {
        public readonly string ParsedPattern;

        /// <summary>
        ///     Supported pattern placeholders:
        ///     {date:format}
        ///     {logger}
        ///     {level}
        ///     {message}
        ///     {threadId}
        /// </summary>
        public MessageLayout(string pattern)
        {
            bool inPlaceholderName = false;
            var parsedPattern = new StringBuilder(pattern.Length);
            StringBuilder name = null;
            using (var e = pattern.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    var c = e.Current;
                    if (c == '{')
                    {
                        inPlaceholderName = true;
                        name = new StringBuilder();
                    }
                    else if (c == '}' || c == ':')
                    {
                        inPlaceholderName = false;
                        
                        var index = GetPlaceholderIndex(name);
                        if (index >= 0)
                        {
                            name = null;
                            parsedPattern.Append(index);
                        }
                    }

                    if (inPlaceholderName && c != '{')
                        name.Append(c);
                    else
                        parsedPattern.Append(c);
                }
            }
            ParsedPattern = parsedPattern.ToString();
        }

        private static int GetPlaceholderIndex(StringBuilder nameBuilder)
        {
            if (nameBuilder == null)
                return -1;

            var name = nameBuilder.ToString();
            var trimmed = name.Trim().ToLowerInvariant();
            switch (trimmed)
            {
                case "message":
                    return 0;
                case "logger":
                    return 1;
                case "level":
                    return 2;
                case "date":
                    return 3;
                case "threadid":
                    return 4;
            }

            throw new ArgumentException("No such placeholder supported: {" + name + "}");
        }

        public string Render(object message, string logger, LogLevel level, DateTime time)
        {
            return string.Format(CultureInfo.InvariantCulture,
                ParsedPattern,
                message, logger, level, time,
                new LazyString(() => Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture)));
        }

        class LazyString
        {
            private readonly Func<string> _getValue;

            public LazyString(Func<string> getValue)
            {
                _getValue = getValue;
            }

            public override string ToString()
            {
                return _getValue();
            }
        }
    }
}