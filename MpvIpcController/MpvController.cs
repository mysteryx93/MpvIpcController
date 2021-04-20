using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        private readonly ManualResetEvent _waitResponse = new ManualResetEvent(true);
        // private int _responseTimeout = 3000;
        private bool _logEnabled;
        private readonly object _lockLogEnabled = new object();
        private readonly SemaphoreSlim _semaphoreSendMessage = new SemaphoreSlim(1, 1);
        private readonly PipeStreamListener _listener;
        private const bool DefaultWaitForResponse = true;
        private const int DefaultResponseTimeout = 3000;
        private const bool DefaultThrowOnError = false;

        /// <summary>
        /// Gets or sets the default options for all requests passing through this controller.
        /// </summary>
        public ApiOptions DefaultOptions { get; } = new ApiOptions()
        {
            WaitForResponse = DefaultWaitForResponse,
            ResponseTimeout = DefaultResponseTimeout,
            ThrowOnError = DefaultThrowOnError
        };

        /// <summary>
        /// Gets whether to wait for response, first taking the value in options, if null taking the value in DefaultOptions, and if null taking a default value.
        /// </summary>
        /// <param name="options">Optional command options, may be null.</param>
        public bool GetWaitForResponse(ApiOptions? options) => options?.WaitForResponse ?? DefaultOptions.WaitForResponse ?? DefaultWaitForResponse;

        /// <summary>
        /// Gets the response timeout, first taking the value in options, if null taking the value in DefaultOptions, and if null taking a default value.
        /// </summary>
        /// <param name="options">Optional command options, may be null.</param>
        public int GetResponseTimeout(ApiOptions? options) => options?.ResponseTimeout ?? DefaultOptions.ResponseTimeout ?? DefaultResponseTimeout;

        /// <summary>
        /// Gets whether to throw an exception on error, first taking the value in options, if null taking the value in DefaultOptions, and if null taking a default value.
        /// </summary>
        /// <param name="options">Optional command options, may be null.</param>
        public bool GetThrowOnError(ApiOptions? options) => options?.ThrowOnError ?? DefaultOptions.ThrowOnError ?? DefaultThrowOnError;

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

            var msg = ParseMessage(message);
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
        /// Sends specified message to MPV and returns a value of specified type.
        /// </summary>
        /// <typeparam name="T">The response data type.</typeparam>
        /// <param name="options">Additional command options.</param>
        /// <param name="cmd">The command values to send.</param>
        /// <returns>The server's response to the command.</returns>
        /// <exception cref="InvalidOperationException">The response contained an error and ThrowOnError is True.</exception>
        /// <exception cref="TimeoutException">A response from MPV was not received before timeout.</exception>
        /// <exception cref="FormatException">The data returned by the server could not be parsed.</exception>
        /// <exception cref="ObjectDisposedException">The underlying connection was disposed.</exception>
        public async Task<MpvResponse<T>> SendMessageAsync<T>(ApiOptions? options, params object?[] cmd)
        {
            var result = await SendMessageAsync(options, cmd).ConfigureAwait(false);
            return result.Parse<T>();
        }

        /// <summary>
        /// Sends specified message to MPV and returns the response as string.
        /// </summary>
        /// <param name="options">Additional command options.</param>
        /// <param name="cmd">The command values to send.</param>
        /// <returns>The server's response to the command.</returns>
        /// <exception cref="InvalidOperationException">The response contained an error and ThrowOnError is True.</exception>
        /// <exception cref="TimeoutException">A response from MPV was not received before timeout.</exception>
        /// <exception cref="FormatException">The data returned by the server could not be parsed.</exception>
        /// <exception cref="ObjectDisposedException">The underlying connection was disposed.</exception>
        public async Task<MpvResponse> SendMessageAsync(ApiOptions? options, params object?[] cmd)
        {
            cmd.CheckNotNullOrEmpty(nameof(cmd));

            // Append prefixes and remove null values at the end.
            var cmdLength = cmd.Length;
            var prefixes = options != null ? options.GetPrefixes() : DefaultOptions.GetPrefixes();
            var prefixCount = prefixes?.Count ?? 0;
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
                    cmd[i] = prefixes![i];
                }
                Array.Copy(cmd2, 0, cmd, prefixCount, cmdLength);
            }

            return await SendMessageNamedAsync(options, cmd, cmd[0]!.ToString()).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends specified message to MPV and returns a value of specified type.
        /// </summary>
        /// <typeparam name="T">The response data type.</typeparam>
        /// <param name="options">Additional command options.</param>
        /// <param name="cmd">The command object to send. It can be an array of parameters or a named object.</param>
        /// <param name="commandName">The name of the command being executed. Only used for debugging.</param>
        /// <returns>The server's response to the command.</returns>
        /// <exception cref="InvalidOperationException">The response contained an error and ThrowOnError is True.</exception>
        /// <exception cref="TimeoutException">A response from MPV was not received before timeout.</exception>
        /// <exception cref="FormatException">The data returned by the server could not be parsed.</exception>
        /// <exception cref="ObjectDisposedException">The underlying connection was disposed.</exception>
        public async Task<MpvResponse<T>> SendMessageNamedAsync<T>(ApiOptions? options, object cmd, string commandName)
        {
            var result = await SendMessageNamedAsync(options, cmd, commandName).ConfigureAwait(false);
            return result.Parse<T>();
        }

        /// <summary>
        /// Sends specified message to MPV and returns the response as string.
        /// </summary>
        /// <param name="options">Additional command options.</param>
        /// <param name="cmd">The command object to send. It can be an array of parameters or a named object.</param>
        /// <param name="commandName">The name of the command being executed. Only used for debugging.</param>
        /// <returns>The server's response to the command.</returns>
        /// <exception cref="InvalidOperationException">The response contained an error and ThrowOnError is True.</exception>
        /// <exception cref="TimeoutException">A response from MPV was not received before timeout.</exception>
        /// <exception cref="FormatException">The data returned by the server could not be parsed.</exception>
        /// <exception cref="ObjectDisposedException">The underlying connection was disposed.</exception>
        public async Task<MpvResponse> SendMessageNamedAsync(ApiOptions? options, object cmd, string commandName)
        {
            cmd.CheckNotNull(nameof(cmd));

            // Prepare the request.
            var request = new MpvRequest()
            {
                Command = cmd,
                RequestId = GetWaitForResponse(options) ? (int?)_requestId++ : null
            };
            var jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = new MpvJsonNamingPolicy(),
                IgnoreNullValues = true
            };
            var jsonString = System.Text.Json.JsonSerializer.Serialize(request, jsonOptions) + '\n';
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
                return new MpvResponse();
            }

            return await WaitForResponseAsync(request.RequestId!.Value, commandName, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Waits for a response with specified request ID.
        /// </summary>
        /// <param name="requestId">The request ID to wait for.</param>
        /// <param name="commandName">The name of the command being executed.</param>
        /// <param name="options">Additional command options.</param>
        private async Task<MpvResponse> WaitForResponseAsync(int requestId, string commandName, ApiOptions? options = null, CancellationToken? cancelToken = null)
        {
            // Wait for response with matching RequestId.
            var watch = new Stopwatch();
            watch.Start();
            var response = FindResponse(requestId);
            var maxTimeout = GetResponseTimeout(options);
            while (response == null && (maxTimeout < 0 || watch.ElapsedMilliseconds < maxTimeout) && cancelToken?.IsCancellationRequested != true)
            {
                // Calculate wait timeout.
                var timeout = -1;
                if (maxTimeout > -1)
                {
                    timeout = (int)(maxTimeout - watch.ElapsedMilliseconds);
                    timeout = timeout < 0 ? 0 : timeout > 1000 ? 1000 : timeout;
                }

                // Wait until any message is received.
                _waitResponse.Reset();
                await _waitResponse.WaitOneAsync(timeout, cancelToken).ConfigureAwait(false);
                response = FindResponse(requestId);
            }

            // Timeout.
            if (response == null)
            {
                throw new TimeoutException($"A response from MPV to request_id={requestId} was not received before timeout.");
            }
            else
            {
                // Remove response from list.
                lock (_responses)
                {
                    _responses.Remove(response);
                }
            }

            if (GetThrowOnError(options) && !response.Success)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Command '{0}' returned status '{1}'.", commandName, response.Error));
            }
            return response;
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

        /// <summary>
        /// Parses a MPV message and returns either a MpvJsonEvent or MpvJsonResponse.
        /// </summary>
        /// <param name="message">The JSON message to parse.</param>
        /// <returns>An MpvJsonEvent or MpvJsonResponse containing the strongly-typed message content.</returns>
        private static object? ParseMessage(string message)
        {
            // using var reader = new JsonTextReader(new StringReader(message));
            var reader = JsonDocument.Parse(message);
            if (reader.RootElement.TryGetProperty("event", out var eventName))
            {
                // Event.
                // ex: { "event": "event_name" }
                var response = new MpvEvent()
                {
                    Event = eventName.GetString() ?? string.Empty
                };
                // Parse additional event args.
                foreach (var item in reader.RootElement.EnumerateObject())
                {
                    if (item.Name != "event")
                    {
                        response.Data.Add(item.Name, item.Value.ToStringInvariant());
                    }
                }
                return response;
            }
            else if (reader.RootElement.TryGetProperty("request_id", out var requestId))
            {
                // Response to a message.
                // ex: { "error": "success", "data": 1.468135, "request_id": 100 }
                var response = new MpvResponse()
                {
                    RequestID = requestId.GetInt32()
                };
                if (reader.RootElement.TryGetProperty("error", out var error))
                {
                    response.Error = error.GetString() ?? string.Empty;
                }
                if (reader.RootElement.TryGetProperty("data", out var data))
                {
                    response.Data = data.ValueKind == JsonValueKind.Null ? null : data.GetRawText();
                }
                return response;
            }
            else
            {
                throw new InvalidDataException($"Unrecognized message: {message}");
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
