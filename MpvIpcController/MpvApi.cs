using System;

// MPV JSON IPC protocol documentation
// https://mpv.io/manual/stable/#json-ipc
//
// Based on v0.32
//
// Changelog to update for future versions available here
// https://github.com/mpv-player/mpv/releases
// https://github.com/mpv-player/mpv/blob/master/DOCS/client-api-changes.rst

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Exposes MPV's API in a strongly-typed way.
    /// </summary>
    public partial class MpvApi : IDisposable
    {
        private readonly IMpvController _mpv;

        public MpvApi(IMpvController controller)
        {
            _mpv = controller;
            _mpv.EventReceived += Mpv_EventReceived;
        }

        /// <summary>
        /// Gets the controller handling API requests.
        /// </summary>
        public IMpvController Controller => _mpv;


        // This is a partial class. See members implementations in
        // MpvApi_Commands
        // MpvApi_Events
        // MpvApi_Properties
        // MpvApi_Options


        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _mpv.Dispose();
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
