using System;
using System.IO;
using JetBrains.Annotations;

namespace LogCast.Owin
{
    /// <summary>
    /// When data is read from, of written to the InnerStream, it is also 
    /// written to the CopyStream.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class InterceptionStream : Stream
    {
        public Stream InnerStream { get; }
        public Stream CopyStream { get; }

        public InterceptionStream(Stream innerStream, Stream copyStream)
        {
            if (copyStream == null)
                throw new ArgumentNullException(nameof(copyStream));

            if (!copyStream.CanWrite)
                throw new ArgumentException("copyStream is not writable");

            InnerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
            CopyStream = copyStream;
        }

        public override void Flush()
        {
            InnerStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return InnerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            InnerStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = InnerStream.Read(buffer, offset, count);

            if (bytesRead != 0)
            {
                CopyStream.Write(buffer, offset, bytesRead);
            }
            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            InnerStream.Write(buffer, offset, count);
            CopyStream.Write(buffer, offset, count);
        }

        public override bool CanRead => InnerStream.CanRead;

        public override bool CanSeek => InnerStream.CanSeek;

        public override bool CanWrite => InnerStream.CanWrite;

        public override long Length => InnerStream.Length;

        public override long Position
        {
            get => InnerStream.Position;
            set => InnerStream.Position = value;
        }

        protected override void Dispose(bool disposing)
        {
            InnerStream.Dispose();
        }
    }
}