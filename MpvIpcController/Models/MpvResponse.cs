using System;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a request to be sent to MPV.
    /// </summary>
    public class MpvResponse
    {
        public string Error { get; set; } = string.Empty;
        public string? Data { get; set; }
        public int? RequestID { get; set; }
    }
}
