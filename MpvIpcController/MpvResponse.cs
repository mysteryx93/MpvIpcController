using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a request to be sent to MPV.
    /// </summary>
    public class MpvResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public object? Data { get; set; }
        [JsonPropertyName("request_id")]
        public int? RequestID { get; set; }
    }
}
