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
        public MpvPropertyRead<int> Count => new(_api, _prefix + "/count");

        /// <summary>
        /// Human-friendy description of the pass.
        /// </summary>
        public MpvPropertyIndexReadRef<int, string> Description => new(_api, _prefix + "/{0}/desc");

        /// <summary>
        /// Last measured execution time, in nanoseconds.
        /// </summary>
        public MpvPropertyIndexRead<int, long> Last => new(_api, _prefix + "/{0}/last");

        /// <summary>
        /// Average execution time of this pass, in nanoseconds. The exact timeframe varies, but it should generally be a handful of seconds.
        /// </summary>
        public MpvPropertyIndexRead<int, long> Avg => new(_api, _prefix + "/{0}/avg");

        /// <summary>
        /// The peak execution time (highest value) within this averaging range, in nanoseconds.
        /// </summary>
        public MpvPropertyIndexRead<int, long> Peak => new(_api, _prefix + "/{0}/peak");

        /// <summary>
        /// The number of samples for this pass.
        /// </summary>
        public MpvPropertyIndexRead<int, int> SamplesCount => new(_api, _prefix + "/{0}/count");

        /// <summary>
        /// The raw execution time of a specific sample for this pass, in nanoseconds.
        /// </summary>
        //public MpvPropertySubIndexRead<int, long> Sample => new(_api, _prefix + "/{0}/samples/{1}");
    }
}
