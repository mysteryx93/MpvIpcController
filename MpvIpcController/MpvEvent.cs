using System;
using System.Collections.Generic;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a request to be sent to MPV.
    /// </summary>
    public class MpvEvent
    {
        public string Event { get; set; } = string.Empty;
        public IDictionary<string, object?> Data { get; } = new Dictionary<string, object?>();
    }
}
