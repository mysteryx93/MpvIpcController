using System;
using System.Collections.Generic;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Model to deserialize demuxer-cache-state property.
    /// </summary>
    public class DemuxerCacheState
    {
        // {"cache-end":0.920000,"reader-pts":-0.080000,"cache-duration":1.000000,"eof":false,"underrun":false,"idle":true,
        // "total-bytes":30256,"fw-bytes":30256,"raw-input-rate":6571,"debug-low-level-seeks":0,"debug-byte-level-seeks":2,
        // "debug-ts-last":0.920000,"bof-cached":false,"eof-cached":false,"seekable-ranges":[]}

        public double CacheEnd { get; set; }
        public double ReaderPts { get; set; }
        public double CacheDuration { get; set; }
        public bool Eof { get; set; }
        public bool Underrun { get; set; }
        public bool Idle { get; set; }
        public long TotalBytes { get; set; }
        public long FwBytes { get; set; }
        public long RawInputRate { get; set; }
        public int DebugLowLevelSeeks { get; set; }
        public int DebugByteLevelSeeks { get; set; }
        public double DebugTsLast { get; set; }
        public bool BofCached { get; set; }
        public bool EndCached { get; set; }
        public IList<DemuxerCacheRange> SeekableRanges { get; } = new List<DemuxerCacheRange>();
    }

    public class DemuxerCacheRange
    {
        public double Start { get; set; }
        public double End { get; set; }
    }
}
