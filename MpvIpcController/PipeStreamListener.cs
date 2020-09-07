using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public delegate void MessageReceived(string message);

    /// <summary>
    /// Listens to messages over a PipeStream and invokes the callback after '\n' is received.
    /// </summary>
    public class PipeStreamListener : IDisposable
    {
        private readonly PipeStream _connection;
        private readonly MessageReceived _callback;
        private readonly int _bufferSize = 1024;
        private readonly byte[] _readBuffer;
        private int _readBufferPos;
        private bool _isStarted;
        private readonly object _lockStart = new object();
        private CancellationTokenSource? _cancelToken;

        /// <summary>
        /// Initializes a new instance of PipeStreamListener.
        /// </summary>
        /// <param name="connection">The PipeStream connection to listen to.</param>
        /// <param name="callback">The callback to invoke after each received message.</param>
        /// <param name="maxMessageLength">The size of the buffer to receive messages.</param>
        public PipeStreamListener(PipeStream connection, MessageReceived callback, int maxMessageLength = 1024)
        {
            _connection = connection;
            _callback = callback;
            _bufferSize = maxMessageLength;
            _readBuffer = new byte[_bufferSize];
        }

        /// <summary>
        /// Starts listening to the PipeStream.
        /// </summary>
        public void Start()
        {
            _ = StartAsync();
        }

        /// <summary>
        /// Starts listening to the PipeStream. This async method won't return as long as the listener is running.
        /// </summary>
        public async Task StartAsync()
        {
            lock (_lockStart)
            {
                if (_isStarted) { throw new InvalidOperationException("The listener is already running."); }
                _isStarted = true;
                _cancelToken = new CancellationTokenSource();
            }

            while (_cancelToken != null)
            {
                var bytesRead = await _connection.ReadAsync(_readBuffer, _readBufferPos, 1, _cancelToken.Token).ConfigureAwait(false);
                if (bytesRead == 1)
                {
                    ByteReceived();
                }
            }

            lock (_lockStart)
            {
                _isStarted = false;
                _cancelToken?.Dispose();
                _cancelToken = null;
            }
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        public void Stop()
        {
            if (_isStarted)
            {
                lock (_lockStart)
                {
                    if (_isStarted)
                    {
                        _isStarted = false;
                        _cancelToken!.Cancel();
                        _cancelToken.Dispose();
                        _cancelToken = null;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs for each byte received from the connection stream.
        /// We must read byte per byte until we find '\n' since we don't know the message length.
        /// </summary>
        private void ByteReceived()
        {
            if (_readBuffer[_readBufferPos++] == '\n')
            {
                // Full message is received.
                var message = Encoding.UTF8.GetString(_readBuffer, 0, _readBufferPos);
                _readBufferPos = 0;
                _callback?.Invoke(message);
            }
            else if (_readBufferPos > _bufferSize)
            {
                throw new IOException("Read buffer is full.");
            }
        }


        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
