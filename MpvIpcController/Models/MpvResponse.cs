using System.Diagnostics.CodeAnalysis;

namespace HanumanInstitute.MpvIpcController;

/// <summary>
/// Represents a response received from MPV, holding Data as raw string.
/// </summary>
public class MpvResponse : MpvResponse<string?>
{
}

/// <summary>
/// Represents a response received from MPV.
/// </summary>
/// <typeparam name="T">The type of returned data.</typeparam>
public class MpvResponse<T>
{
    public string Error { get; set; } = string.Empty;
    [AllowNull]
    public T Data { get; set; } = default!;
    public int? RequestID { get; set; }

    /// <summary>
    /// Returns whether the result is valid and contains data.
    /// </summary>
    public bool HasValue => Data != null && Success;

    /// <summary>
    /// Returns whether the result status is success.
    /// </summary>
    public bool Success => Error == "success";
}