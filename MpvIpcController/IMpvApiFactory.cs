namespace HanumanInstitute.MpvIpcController;

/// <summary>
/// Establishes the connection to a MPV server and creates controllers.
/// </summary>
public interface IMpvApiFactory
{
    /// <summary>
    /// Gets or sets the name of the server to connect to. By default, '.' for local machine.
    /// </summary>
    string ServerName { get; set; }
    /// <summary>
    /// Gets or sets the IPC connection timeout.
    /// </summary>
    int Timeout { get; set; }
    /// <summary>
    /// Starts MPV with specified IPC pipe name and connects to it.
    /// </summary>
    /// <param name="mpvPath">The path to MPV.exe</param>
    /// <param name="pipeName">The name of the IPC pipe name.</param>
    /// <returns>A connected MpvController.</returns>
    /// <exception cref="Win32Exception">An error occurred when opening the associated file.</exception>
    Task<MpvApi> StartAsync(string mpvPath, string pipeName = "mpvpipe");
    /// <summary>
    /// Connects to an existing instance of MPV via specified IPC pipe name.
    /// </summary>
    /// <param name="pipeName">The IPC pipe name to connect to.</param>
    /// <returns>A connected MpvController.</returns>
    Task<MpvApi> ConnectAsync(string pipeName);
}