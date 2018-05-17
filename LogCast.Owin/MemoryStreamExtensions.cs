using System;
using System.IO;

namespace LogCast.Owin
{
    internal static  class MemoryStreamExtensions
    {
        internal static string ReadAsString(this MemoryStream stream)
        {
            if (stream.Length == 0)
            {
                return null;
            }
            if (!stream.CanRead)
            {
                throw new Exception("MemoryStream is not readable");
            }
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
