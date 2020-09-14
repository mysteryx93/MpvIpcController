using System;
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
        /// Gets or sets the default options for all requests passing through this controller.
        /// </summary>
        ApiOptions DefaultOptions { get; }
        /// <summary>
        /// Gets whether to wait for response, first taking the value in options, if null taking the value in DefaultOptions, and if null taking a default value.
        /// </summary>
        /// <param name="options">Optional command options, may be null.</param>
        bool GetWaitForResponse(ApiOptions? options);
        /// <summary>
        /// Gets the response timeout, first taking the value in options, if null taking the value in DefaultOptions, and if null taking a default value.
        /// </summary>
        /// <param name="options">Optional command options, may be null.</param>
        int GetResponseTimeout(ApiOptions? options);
        /// <summary>
        /// Gets whether to throw an exception on error, first taking the value in options, if null taking the value in DefaultOptions, and if null taking a default value.
        /// </summary>
        /// <param name="options">Optional command options, may be null.</param>
        bool GetThrowOnError(ApiOptions? options);
        /// <summary>
        /// Gets or sets whether to keep a log of communication data.
        /// </summary>
        bool LogEnabled { get; set; }
        /// <summary>
        /// Gets a text log of communication data from both directions.
        /// </summary>
        StringBuilder? Log { get; }
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
        Task<MpvResponse<T>?> SendMessageAsync<T>(ApiOptions? options, params object?[] cmd);
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
        Task<MpvResponse?> SendMessageAsync(ApiOptions? options, params object?[] cmd);
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
        Task<MpvResponse<T>?> SendMessageNamedAsync<T>(ApiOptions? options, object cmd, string commandName);
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
        Task<MpvResponse?> SendMessageNamedAsync(ApiOptions? options, object cmd, string commandName);
    }
}
