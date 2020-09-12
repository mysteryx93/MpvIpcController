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
        public MpvPropertyRead<string?> Format => _format ??= new MpvPropertyRead<string?>(_api, _prefix + "/format");
        private MpvPropertyRead<string?>? _format;

        /// <summary>
        /// The audio sample rate.
        /// </summary>
        public MpvPropertyRead<int?> SampleRate => _sampleRate ??= new MpvPropertyRead<int?>(_api, _prefix + "/samplerate");
        private MpvPropertyRead<int?>? _sampleRate;

        /// <summary>
        /// The channel layout as a string. This is similar to what the --audio-channels accepts.
        /// </summary>
        public MpvPropertyRead<string?> AudioChannels => _channels ??= new MpvPropertyRead<string?>(_api, _prefix + "/channels");
        private MpvPropertyRead<string?>? _channels;

        /// <summary>
        /// As channels, but instead of the possibly cryptic actual layout sent to the audio device, return a hopefully more human readable form. (Usually only AudioOutParams.HrChannels makes sense.)
        /// </summary>
        public MpvPropertyRead<string?> HrChannels => _hrChannels ??= new MpvPropertyRead<string?>(_api, _prefix + "/hr-channels");
        private MpvPropertyRead<string?>? _hrChannels;

        /// <summary>
        /// Number of audio channels.
        /// </summary>
        public MpvPropertyRead<int?> ChannelCount => _channelCount ??= new MpvPropertyRead<int?>(_api, _prefix + "/channel-count");
        private MpvPropertyRead<int?>? _channelCount;
    }
}
