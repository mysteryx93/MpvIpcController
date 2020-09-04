using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Handles basic communication protocols with MPV via IPC named pipe.
    /// </summary>
    public class MpvController : IDisposable, IMpvController
    {
        private readonly Stream _connection;
        private const int InBufferSize = 1024;
        private readonly byte[] _readBuffer = new byte[InBufferSize];
        private int _readBufferPos;
        private int _requestId;
        private readonly List<MpvResponse> _responses = new List<MpvResponse>();
        private readonly ManualResetEventSlim _waitResponse = new ManualResetEventSlim(true);

        /// <summary>
        /// Occurs when an event is received.
        /// </summary>
        public event EventHandler<MpvMessageEventArgs>? EventReceived;

        /// <summary>
        /// Initializes a new instance of the MpvController class to handle communication over specified stream.
        /// </summary>
        /// <param name="connection">A stream supporting both reading and writing.</param>
        public MpvController(Stream connection)
        {
            _connection = connection.CheckNotNull(nameof(connection));
            if (!_connection.CanRead || !_connection.CanWrite)
            {
                throw new ArgumentException("Connection must support both reading and writing.");
            }

            BeginReadLoop();
        }

        /// <summary>
        /// Gets or sets the timeout in milliseconds to wait for a message response.
        /// </summary>
        public int ResponseTimeout
        {
            get => _responseTimeout;
            set => _responseTimeout = value >= 0 ? value : -1;
        }
        private int _responseTimeout = 3000;

        /// <summary>
        /// Starts listening to messages over the connection stream.
        /// </summary>
        private void BeginReadLoop()
        {
            _connection!.BeginRead(_readBuffer, _readBufferPos, 1, DataReceived, null);
        }

        /// <summary>
        /// Occurs for each byte received from the connection stream.
        /// We must read byte per byte until we find '\n' since we don't know the message length.
        /// </summary>
        private void DataReceived(IAsyncResult ar)
        {
            var bytesRead = _connection!.EndRead(ar);
            // Stop reading if we receive no data.
            if (bytesRead == 1)
            {
                if (_readBuffer[_readBufferPos++] == '\n')
                {
                    // Full message is received.
                    var message = Encoding.UTF8.GetString(_readBuffer, 0, _readBufferPos);
                    _readBufferPos = 0;
                    MessageReceived(message);
                }
                else if (_readBufferPos > InBufferSize)
                {
                    throw new IOException("Read buffer is full.");
                }

                // Continue reading in loop.
                BeginReadLoop();
            }
        }

        /// <summary>
        /// Occurs when a full message has been received.
        /// </summary>
        /// <param name="message">The message received from the server in JSON format.</param>
        private void MessageReceived(string message)
        {
            if (string.IsNullOrEmpty(message)) { return; }

            var msg = MpvParser.Parse(message);
            if (msg is MpvEvent msgEvent)
            {
                // Raise event.
                EventReceived?.Invoke(this, new MpvMessageEventArgs(msgEvent));
            }
            else if (msg is MpvResponse msgResponse)
            {
                // Add to list of responses to be retrieved by QueryId.
                _responses.Add(msgResponse);
                _waitResponse.Set();
            }
        }

        /// <summary>
        /// Sends specified message to MPV.
        /// </summary>
        /// <param name="commandName">The command to send.</param>
        /// <param name="args">Additional command parameters.</param>
        /// <returns>The server's response to the command.</returns>
        public async Task<MpvResponse> SendMessageAsync(string commandName, params object[] args)
        {
            var cmd = new object[args.Length + 1];
            cmd[0] = commandName;
            if (args.Length > 0)
            {
                args.CopyTo(cmd, 1);
            }

            var request = new MpvRequest()
            {
                Command = cmd,
                RequestId = _requestId++
            };
            var jsonString = System.Text.Json.JsonSerializer.Serialize(request) + '\n';
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            await _connection.WriteAsync(jsonBytes, 0, jsonBytes.Length).ConfigureAwait(false);

            // Wait for response with matching RequestId.
            var watch = new Stopwatch();
            watch.Start();
            var responseIndex = FindResponseIndex(request.RequestId.Value);
            while (responseIndex < 0 && watch.ElapsedMilliseconds < ResponseTimeout)
            {
                // Calculate wait timeout.
                var timeout = -1;
                if (ResponseTimeout > -1)
                {
                    timeout = (int)(ResponseTimeout - watch.ElapsedMilliseconds);
                    timeout = timeout < 0 ? 0 : timeout;
                }

                // Wait until any message is received.
                _waitResponse.Reset();
                _waitResponse.Wait(timeout);
                responseIndex = FindResponseIndex(request.RequestId.Value);
            }

            // Timeout.
            if (responseIndex < 0)
            {
                throw new TimeoutException("A command response from MPV was not received before timeout.");
            }

            // Remove response from list and return.
            var response = _responses[responseIndex];
            _responses.RemoveAt(responseIndex);
            return response;
        }

        /// <summary>
        /// Returns the index at which specified request id is found in the list of responses, or -1.
        /// </summary>
        /// <param name="requestId">The request id to look for.</param>
        /// <returns>The position of the response in the list, or -1.</returns>
        private int FindResponseIndex(int requestId)
        {
            for (var i = 0; i < _responses.Count; i++)
            {
                if (_responses[i].RequestID == requestId)
                {
                    return i;
                }
            }
            return -1;
        }


        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _connection?.Dispose();
                    _waitResponse.Dispose();
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
