using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class MpvControllerTests : IDisposable
    {
        public Stream Connection => _connection ??= new MemoryStream();
        private MemoryStream? _connection;

        public IMpvController Model => _model ??= new MpvController(Connection);
        private IMpvController? _model;

        [Fact]
        public async Task ConnectAsync()
        {

        }


        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _connection?.Dispose();
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
