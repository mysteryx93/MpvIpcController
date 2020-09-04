using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Establishes the connection to a MPV server and creates controllers.
    /// </summary>
    public class MpvControllerFactory : IMpvControllerFactory
    {
        /// <summary>
        /// Gets or sets the name of the server to connect to. By default, '.' for local machine.
        /// </summary>
        public string ServerName
        {
            get => _serverName;
            set => _serverName = !string.IsNullOrEmpty(_serverName) ? _serverName : ".";
        }
        private string _serverName = ".";

        /// <summary>
        /// Gets or sets the IPC connection timeout.
        /// </summary>
        public int Timeout
        {
            get => _timeout;
            set => _timeout = value >= 0 ? value : -1;
        }
        private int _timeout = 5000;

        /// <summary>
        /// Starts MPV with specified IPC pipe name and connects to it.
        /// </summary>
        /// <param name="mpvPath">The path to MPV.exe</param>
        /// <param name="pipeName">The name of the IPC pipe name.</param>
        /// <returns>A connected MpvController.</returns>
        /// <exception cref="Win32Exception">An error occurred when opening the associated file.</exception>
        public async Task<IMpvController> StartAsync(string mpvPath, string pipeName = "mpvpipe")
        {
            mpvPath.CheckNotNullOrEmpty(nameof(mpvPath));

            Process.Start(mpvPath, $"--input-ipc-server={pipeName}");

            return await ConnectAsync(pipeName).ConfigureAwait(false);
        }


        /// <summary>
        /// Connects to an existing instance of MPV via specified IPC pipe name.
        /// </summary>
        /// <param name="pipeName">The IPC pipe name to connect to.</param>
        /// <returns>A connected MpvController.</returns>
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed: Connection closure is handled by MpvController.")]
        public async Task<IMpvController> ConnectAsync(string pipeName)
        {
            var connection = new NamedPipeClientStream(_serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            await connection.ConnectAsync(Timeout).ConfigureAwait(false);

            if (!connection.IsConnected || !connection.CanRead || !connection.CanWrite)
            {
                connection.Dispose();
                throw new InvalidOperationException("Cannot connect to the MPC IPC socket.");
            }

            return new MpvController(connection);
        }
    }
}
