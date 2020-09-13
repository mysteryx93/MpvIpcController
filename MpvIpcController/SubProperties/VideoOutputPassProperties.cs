using System;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Exposes Video Output Pass sub-properties.
    /// </summary>
    public class VideoOutputPassProperties
    {
        private readonly MpvApi _api;
        private readonly string _prefix;

        public VideoOutputPassProperties(MpvApi api, string propertyName)
        {
            _api = api;
            _prefix = propertyName;
        }

        /// <summary>
        /// Number of passes.
        /// </summary>
        public MpvPropertyRead<int?> Count => _count ??= new MpvPropertyRead<int?>(_api, _prefix + "/count");
        private MpvPropertyRead<int?>? _count;

        /// <summary>
        /// Human-friendy description of the pass.
        /// </summary>
        public MpvPropertyIndexRead<int, string?> Description => _description ??= new MpvPropertyIndexRead<int, string?>(_api, _prefix + "/{0}/desc");
        private MpvPropertyIndexRead<int, string?>? _description;

        /// <summary>
        /// Last measured execution time, in nanoseconds.
        /// </summary>
        public MpvPropertyIndexRead<int, long?> Last => _last ??= new MpvPropertyIndexRead<int, long?>(_api, _prefix + "/{0}/last");
        private MpvPropertyIndexRead<int, long?>? _last;

        /// <summary>
        /// Average execution time of this pass, in nanoseconds. The exact timeframe varies, but it should generally be a handful of seconds.
        /// </summary>
        public MpvPropertyIndexRead<int, long?> Avg => _avg ??= new MpvPropertyIndexRead<int, long?>(_api, _prefix + "/{0}/avg");
        private MpvPropertyIndexRead<int, long?>? _avg;

        /// <summary>
        /// The peak execution time (highest value) within this averaging range, in nanoseconds.
        /// </summary>
        public MpvPropertyIndexRead<int, long?> Peak => _peak ??= new MpvPropertyIndexRead<int, long?>(_api, _prefix + "/{0}/peak");
        private MpvPropertyIndexRead<int, long?>? _peak;

        /// <summary>
        /// The number of samples for this pass.
        /// </summary>
        public MpvPropertyIndexRead<int, int?> SamplesCount => _samplesCount ??= new MpvPropertyIndexRead<int, int?>(_api, _prefix + "/{0}/count");
        private MpvPropertyIndexRead<int, int?>? _samplesCount;

        /// <summary>
        /// The raw execution time of a specific sample for this pass, in nanoseconds.
        /// </summary>
        //public MpvPropertySubIndexRead<int, long?> Sample => _sample ??= new MpvPropertySubIndexRead<int, long?>(_api, _prefix + "/{0}/samples/{1}");
        //private MpvPropertySubIndexRead<int, long?>? _sample;
    }
}
