using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Handles basic communication protocols with MPV via IPC named pipe.
    /// </summary>
    public class MpvController : IDisposable, IMpvController
    {
        private readonly NamedPipeClientStream _connection;
        private int _requestId = 1;
        private readonly List<MpvResponse> _responses = new List<MpvResponse>();
        private readonly ManualResetEventSlim _waitResponse = new ManualResetEventSlim(true);
        private int _responseTimeout = 3000;
        private bool _logEnabled;
        private readonly object _lockLogEnabled = new object();
        private readonly SemaphoreSlim _semaphoreSendMessage = new SemaphoreSlim(1, 1);
        private readonly PipeStreamListener _listener;

        /// <summary>
        /// Gets a text log of communication data from both directions.
        /// </summary>
        public StringBuilder? Log { get; private set; }

        /// <summary>
        /// Occurs when an event is received.
        /// </summary>
        public event EventHandler<MpvMessageEventArgs>? EventReceived;

        /// <summary>
        /// Initializes a new instance of the MpvControllerBase class to handle communication over specified stream.
        /// </summary>
        /// <param name="connection">A stream supporting both reading and writing.</param>
        public MpvController(NamedPipeClientStream connection)
        {
            _connection = connection.CheckNotNull(nameof(connection));
            if (!_connection.CanRead || !_connection.CanWrite)
            {
                throw new ArgumentException("Connection must support both reading and writing.");
            }

            _listener = new PipeStreamListener(connection, MessageReceived);
            _listener.Start();
        }

        /// <summary>
        /// Gets or sets the timeout in milliseconds to wait for a message response.
        /// </summary>
        public int ResponseTimeout
        {
            get => _responseTimeout;
            set => _responseTimeout = value >= 0 ? value : -1;
        }

        /// <summary>
        /// Gets or sets whether to keep a log of communication data.
        /// </summary>
        public bool LogEnabled
        {
            get => _logEnabled;
            set
            {
                lock (_lockLogEnabled)
                {
                    if (value && !_logEnabled)
                    {
                        Log = new StringBuilder();
                    }
                    else if (!value && _logEnabled)
                    {
                        Log = null;
                    }
                    _logEnabled = value;
                }
            }
        }

        /// <summary>
        /// Occurs when a full message has been received.
        /// </summary>
        /// <param name="message">The message received from the server in JSON format.</param>
        private void MessageReceived(string message)
        {
            if (string.IsNullOrEmpty(message)) { return; }

            LogAppend(message);

            var msg = MpvParser.Parse(message);
            if (msg is MpvEvent msgEvent)
            {
                // Raise event.
                EventReceived?.Invoke(this, new MpvMessageEventArgs(msgEvent));
            }
            else if (msg is MpvResponse msgResponse && (msgResponse.RequestID ?? 0) > 0)
            {
                // Add to list of responses to be retrieved by QueryId.
                lock (_responses)
                {
                    _responses.Add(msgResponse);
                }
                _waitResponse.Set();
            }
        }

        /// <summary>
        /// Sends specified message to MPV.
        /// </summary>
        /// <param name="options">Additional command options.</param>
        /// <param name="cmd">The command values to send.</param>
        /// <returns>The server's response to the command.</returns>
        public async Task<object?> SendMessageAsync(MpvCommandOptions? options, params object?[] cmd)
        {
            cmd.CheckNotNullOrEmpty(nameof(cmd));

            // Append prefixes and remove null values at the end.
            var cmdLength = cmd.Length;
            var prefixCount = options?.Prefixes?.Count ?? 0;
            for (var i = cmd.Length - 1; i >= 0; i--)
            {
                if (cmd[i] == null)
                {
                    cmdLength--;
                }
                else
                {
                    break;
                }
            }
            if (cmdLength != cmd.Length || prefixCount > 0)
            {
                var cmd2 = cmd;
                cmd = new object?[cmdLength + prefixCount];
                for (var i = 0; i < prefixCount; i++)
                {
                    cmd[i] = options!.Prefixes![i];
                }
                Array.Copy(cmd2, 0, cmd, prefixCount, cmdLength);
            }

            // Prepare the request.
            var request = new MpvRequest()
            {
                Command = cmd,
                RequestId = (options == null || options.WaitForResponse) ? (int?)_requestId++ : 0
            };
            var jsonString = System.Text.Json.JsonSerializer.Serialize(request) + '\n';
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            // Send the request.
            await _semaphoreSendMessage.WaitAsync().ConfigureAwait(false);
            try
            {
                await _connection.WriteAsync(jsonBytes, 0, jsonBytes.Length).ConfigureAwait(false);
            }
            finally
            {
                _semaphoreSendMessage.Release();
            }
            LogAppend(jsonString);

            if (!request.RequestId.HasValue)
            {
                return null;
            }

            // Wait for response with matching RequestId.
            var watch = new Stopwatch();
            watch.Start();
            var response = FindResponse(request.RequestId.Value);
            var maxTimeout = options?.ResponseTimeout ?? ResponseTimeout;
            while (response == null && (maxTimeout < 0 || watch.ElapsedMilliseconds < maxTimeout))
            {
                // Calculate wait timeout.
                var timeout = 1000;
                if (ResponseTimeout > -1)
                {
                    timeout = (int)(ResponseTimeout - watch.ElapsedMilliseconds);
                    timeout = timeout < 0 ? 0 : timeout > 1000 ? 1000 : timeout;
                }

                // Wait until any message is received.
                _waitResponse.Reset();
                _waitResponse.Wait(timeout);
                response = FindResponse(request.RequestId.Value);
            }

            // Timeout.
            if (response == null)
            {
                throw new TimeoutException($"A response from MPV to request_id={request.RequestId.Value} was not received before timeout.");
            }
            else
            {
                // Remove response from list.
                lock (_responses)
                {
                    _responses.Remove(response);
                }
            }

            if (response.Error != "success")
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Command '{0}' returned status '{1}'.", cmd[0], response.Error));
            }
            return response.Data;
        }

        /// <summary>
        /// Returns the response with specified ID from the list of received responses.
        /// </summary>
        /// <param name="requestId">The request id to look for.</param>
        /// <returns>The response with matching id.</returns>
        private MpvResponse FindResponse(int requestId)
        {
            lock (_responses)
            {
                return _responses.FirstOrDefault(x => x.RequestID == requestId);
            }
        }

        /// <summary>
        /// Adds a message to the log if enabled.
        /// </summary>
        /// <param name="message">The message to append to the log.</param>
        private void LogAppend(string message)
        {
            if (Log != null)
            {
                lock (Log)
                {
                    Log?.Append(message);
                }
            }
        }


        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _listener.Dispose();
                    _connection?.Dispose();
                    _waitResponse.Dispose();
                    _semaphoreSendMessage.Dispose();
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
