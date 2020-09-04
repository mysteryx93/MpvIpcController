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
        public IDictionary<string, object?> Data { get; } = new Dictionary<string, object?>();
        [JsonPropertyName("request_id")]
        public int? RequestID { get; set; }
    }
}
