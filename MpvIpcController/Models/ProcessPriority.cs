using System;
using System.Text.Json.Serialization;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Set process priority for mpv according to the predefined priorities available under Windows.
    /// </summary>
    public enum ProcessPriority
    {
        Idle,
        [JsonPropertyName("belownormal")]
        BelowNormal,
        Normal,
        [JsonPropertyName("abovenormal")]
        AboveNormal,
        High,
        Realtime
    }
}
