using System;
using System.Collections.Generic;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Contains the properties of an audio or video filter.
    /// </summary>
    public class AudioVideoFilter
    {
        public string? Name { get; set; }
        public string? Label { get; set; }
        public bool? Enabled { get; set; }
        public IDictionary<string, string> Params { get; } = new Dictionary<string, string>();
    }
}
