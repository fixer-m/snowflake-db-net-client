using System;
using System.Collections.Generic;
using System.IO;

namespace Snowflake.Client
{
    /// <summary>
    /// Used to merge multiple streams into one
    /// </summary>
    internal class ConcatenatedStream : Stream
    {
        private readonly Queue<Stream> _streams;

        public ConcatenatedStream(IEnumerable<Stream> streams)
        {
            _streams = new Queue<Stream>(streams);
        }

        public override bool CanRead => true;
        
        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_streams.Count == 0)
                return 0;

            var bytesRead = _streams.Peek().Read(buffer, offset, count);
            if (bytesRead == 0)
            {
                _streams.Dequeue().Dispose();
                bytesRead += Read(buffer, offset, count);
            }
            return bytesRead;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length => throw new NotImplementedException();

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public override void SetLength(long value) => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
    }
}
