using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public interface IMpvController : IDisposable
    {
        /// <summary>
        /// Occurs when an event is received.
        /// </summary>
        event EventHandler<MpvMessageEventArgs>? EventReceived;
        /// <summary>
        /// Gets or sets the timeout in milliseconds to wait for a message response.
        /// </summary>
        int ResponseTimeout { get; set; }
        /// <summary>
        /// Gets or sets whether to keep a log of communication data.
        /// </summary>
        bool LogEnabled { get; set; }
        /// <summary>
        /// Gets a text log of communication data from both directions.
        /// </summary>
        StringBuilder? Log { get; }
        /// <summary>
        /// Sends specified message to MPV and returns a nullable value of specified value type.
        /// </summary>
        /// <param name="options">Additional command options.</param>
        /// <param name="cmd">The command values to send.</param>
        /// <returns>The server's response to the command.</returns>
        Task<T?> SendMessageAsync<T>(MpvCommandOptions? options, params object?[] cmd) where T : struct;
        /// <summary>
        /// Sends specified message to MPV and returns a class of specified type.
        /// </summary>
        /// <param name="options">Additional command options.</param>
        /// <param name="cmd">The command values to send.</param>
        /// <returns>The server's response to the command.</returns>
        Task<T?> SendMessageClassAsync<T>(MpvCommandOptions? options, params object?[] cmd) where T : class;
        /// <summary>
        /// Sends specified message to MPV.
        /// </summary>
        /// <param name="options">Additional command options.</param>
        /// <param name="cmd">The command values to send.</param>
        /// <returns>The server's response to the command.</returns>
        Task<string?> SendMessageAsync(MpvCommandOptions? options, params object?[] cmd);
    }
}
