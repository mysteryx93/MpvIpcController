using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class MpvControllerTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public MpvControllerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private const string PipeName = "testpipe";
        [NotNull]
        private NamedPipeServerStream? _server;
        [NotNull]
        private NamedPipeClientStream? _client;
        private readonly byte[] _readBuffer = new byte[1];
        [NotNull]
        private IMpvController? _model;
        private readonly SemaphoreSlim _semaphoreResponse = new SemaphoreSlim(1, 1);

        private void InitConnection()
        {
            _server = new NamedPipeServerStream(PipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances,
                                                    PipeTransmissionMode.Byte,
                                                    PipeOptions.Asynchronous);
            _client = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            _client.Connect();
            _server.WaitForConnection();
            BeginReadLoop();
            _model = new MpvController(_client);
        }

        private void BeginReadLoop()
        {
            _server!.BeginRead(_readBuffer, 0, 1, DataReceived, null);
        }

        private void DataReceived(IAsyncResult ar)
        {
            if (_server.EndRead(ar) > 0)
            {
                BeginReadLoop();
            }
        }

        private async Task WriteServerMessageAsync(string message)
        {
            var data = Encoding.UTF8.GetBytes(message).Append(Convert.ToByte('\n')).ToArray();
            await _semaphoreResponse.WaitAsync().ConfigureAwait(false);
            try
            {
                await _server.WriteAsync(data).ConfigureAwait(false);
            }
            finally
            {
                _semaphoreResponse.Release();
            }
        }

        private const string CommandName = "loadfile";
        private string GetResponseSimple(int requestId = 0) => $@"{{ ""data"": null, ""error"": ""success"", ""request_id"": {requestId} }}";
        private string GetResponseWithInt(int requestId = 0) => $@"{{ ""data"": 15, ""error"": ""success"", ""request_id"": {requestId} }}";
        private string GetResponseWithArray(int requestId = 0) => $@"{{ ""data"": [""a"", ""b""], ""error"": ""success"", ""request_id"": {requestId} }}";
        private const string EventMessage = @"{ ""event"": ""property-change"", ""id"": 1, ""data"": ""52.000000"", ""name"": ""volume"" }";

        [Fact]
        public async Task EventReceived_Triggered_ContainsData()
        {
            InitConnection();

            IDictionary<string, object?>? data = null;
            _model.EventReceived += (s, e) =>
            {
                data = e.Event.Data;
            };
            await WriteServerMessageAsync(EventMessage);
            await Task.Delay(50);

            Assert.NotNull(data);
            Assert.NotEmpty(data);
        }

        [Fact]
        public async Task SendMessage_CommandSimple_ReceivesResponse()
        {
            InitConnection();

            var requestTask = _model.SendMessageAsync(CommandName);
            await WriteServerMessageAsync(GetResponseSimple());
            var response = await requestTask;

            Assert.Null(response);
        }

        [Fact]
        public async Task SendMessage_CommandWithParams_ReceivesResponseWithInt()
        {
            InitConnection();

            var requestTask = _model.SendMessageAsync(CommandName);
            await WriteServerMessageAsync(GetResponseWithInt());
            var response = await requestTask;

            Assert.IsType<int>(response);
        }

        [Fact]
        public async Task SendMessage_CommandWithParams_ReceivesResponseWithArray()
        {
            InitConnection();

            var requestTask = _model.SendMessageAsync(CommandName);
            await WriteServerMessageAsync(GetResponseWithArray());
            var response = await requestTask;

            Assert.NotEmpty((IEnumerable?)response);
        }

        [Fact]
        public async Task SendMessage_TimeoutNegative_DoesNotTimeout()
        {
            InitConnection();
            _model.ResponseTimeout = -1;

            var requestTask = _model.SendMessageAsync(CommandName);
            await Task.Delay(50);
            await WriteServerMessageAsync(GetResponseSimple());
            await requestTask;
        }

        [Fact]
        public async Task SendMessage_TimeoutZero_ThrowsException()
        {
            InitConnection();
            _model.ResponseTimeout = 0;

            async Task Act()
            {
                var requestTask = _model.SendMessageAsync(CommandName);
                await Task.Delay(50);
                await WriteServerMessageAsync(GetResponseSimple());
                var response = await requestTask;
            }

            await Assert.ThrowsAsync<TimeoutException>(Act);
        }

        [Fact]
        public async Task SendMessage_ConcurrentRequests_ReceivesAllResponses()
        {
            InitConnection();
            _model.LogEnabled = true;
            const int Concurrent = 10;
            var requests = new int[Concurrent];
            for (var i = 0; i < Concurrent; i++)
            {
                requests[i] = i;
            }

            try
            {
                var tasksClient = requests.AsyncParallelForEach(x => _model.SendMessageAsync(CommandName));
                await Task.Delay(10);
                var tasksServer = requests.AsyncParallelForEach(x => WriteServerMessageAsync(GetResponseSimple(x)));
                await Task.Delay(100);
                await Task.WhenAll(tasksClient, tasksServer).ConfigureAwait(false);
            }
            finally
            {
                _output.WriteLine(_model?.Log?.ToString());
            }

            // Success: No freeze and no crash.
        }

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _client?.Dispose();
                    _server?.Dispose();
                    _semaphoreResponse.Dispose();
                    _model?.Dispose();
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
