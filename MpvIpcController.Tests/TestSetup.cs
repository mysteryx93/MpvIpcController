using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class TestSetup : IDisposable
    {
        private TestSetup()
        { }

        public static TestSetup Create() => new TestSetup().InitConnection();

        private const string PipeName = "testpipe";
        [NotNull]
        public NamedPipeServerStream? Server { get; private set; }
        [NotNull]
        public NamedPipeClientStream? Client { get; private set; }
        [NotNull]
        public IMpvApi? Model { get; private set; }
        private readonly SemaphoreSlim _semaphoreResponse = new SemaphoreSlim(1, 1);
        [NotNull]
        private PipeStreamListener? _listener;
        public StringBuilder ServerLog { get; } = new StringBuilder();

        private TestSetup InitConnection()
        {
            Server = new NamedPipeServerStream(PipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances,
                                                    PipeTransmissionMode.Byte,
                                                    PipeOptions.Asynchronous);
            Client = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            Client.Connect();
            Server.WaitForConnection();
            _listener = new PipeStreamListener(Server, MessageReceived);
            _listener.Start();
            Model = new MpvApi(Client);
            return this;
        }

        private void MessageReceived(string message)
        {
            ServerLog.Append(message);
        }

        public async Task WriteServerMessageAsync(string message)
        {
            var data = Encoding.UTF8.GetBytes(message).Append(Convert.ToByte('\n')).ToArray();
            await _semaphoreResponse.WaitAsync().ConfigureAwait(false);
            try
            {
                await Server.WriteAsync(data).ConfigureAwait(false);
            }
            finally
            {
                _semaphoreResponse.Release();
            }
        }

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _listener?.Dispose();
                    Model?.Dispose();
                    Client?.Dispose();
                    Server?.Dispose();
                    _semaphoreResponse.Dispose();
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
