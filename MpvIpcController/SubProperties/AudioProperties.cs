using System;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Exposes AudioParams sub-properties.
    /// </summary>
    public class AudioProperties
    {
        private readonly MpvApi _api;
        private readonly string _prefix;

        public AudioProperties(MpvApi api, string propertyName)
        {
            _api = api;
            _prefix = propertyName;
        }

        /// <summary>
        /// The audio sample format as string. This uses the same names as used in other places of mpv.
        /// </summary>
        public MpvPropertyReadString Format => new(_api, _prefix + "/format");

        /// <summary>
        /// The audio sample rate.
        /// </summary>
        public MpvPropertyRead<int> SampleRate => new(_api, _prefix + "/samplerate");

        /// <summary>
        /// The channel layout as a string. This is similar to what the --audio-channels accepts.
        /// </summary>
        public MpvPropertyReadString AudioChannels => new(_api, _prefix + "/channels");

        /// <summary>
        /// As channels, but instead of the possibly cryptic actual layout sent to the audio device, return a hopefully more human readable form. (Usually only AudioOutParams.HrChannels makes sense.)
        /// </summary>
        public MpvPropertyReadString HrChannels => new(_api, _prefix + "/hr-channels");

        /// <summary>
        /// Number of audio channels.
        /// </summary>
        public MpvPropertyRead<int> ChannelCount => new(_api, _prefix + "/channel-count");
    }
}
