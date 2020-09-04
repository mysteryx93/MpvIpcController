using System;
using System.Collections;
using System.Text.Json.Serialization;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a request to be sent to MPV.
    /// </summary>
    public class MpvRequest
    {
        [JsonPropertyName("command")]
        public IEnumerable Command { get; set; } = null!;
        [JsonPropertyName("request_id")]
        public int? RequestId { get; set; }
    }
}
