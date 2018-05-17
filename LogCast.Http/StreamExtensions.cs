using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LogCast.Http
{
    public static class StreamExtensions
    {
        private const int ReadBufferSize = 2048;
        private const int BodyLimit = ReadBufferSize * 5;

        public static async Task<string> ReadSafelyAsync(this Stream stream)
        {
            var validationMessage = Validate(stream);
            if (validationMessage != null)
                return validationMessage;

            var wasntRead = stream.Position == 0;

            string result;
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, ReadBufferSize, true))
            {
                result = await reader.ReadToEndAsync();
            }

            if (wasntRead)
                stream.Seek(0, SeekOrigin.Begin);

            return result;
        }

        public static string ReadSafely(this Stream stream)
        {
            var validationMessage = Validate(stream);
            if (validationMessage != null)
                return validationMessage;

            var wasntRead = stream.Position == 0;

            string result;
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, ReadBufferSize, true))
            {
                result = reader.ReadToEnd();
            }

            if (wasntRead)
                stream.Seek(0, SeekOrigin.Begin);

            return result;
        }


        private static string Validate(Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return "<unavailable>";

            var length = stream.Length;
            if (length > BodyLimit)
                return "<too_big>";

            return null;
        }
    }
}