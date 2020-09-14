using System;

namespace HanumanInstitute.MpvIpcController
{
    public partial class MpvApi
    {
        /// <summary>
        /// Specify a priority list of audio languages to use. Different container formats employ different language codes. DVDs use ISO 639-1 two-letter language codes, Matroska, MPEG-TS and NUT use ISO 639-2 three-letter language codes, while OGM uses a free-form identifier. See also --aid.
        /// </summary>
        public MpvOptionList AudioLanguage => _audioLanguage ??= new MpvOptionList(this, "alang");
        private MpvOptionList? _audioLanguage;
        /// <summary>
        /// Specify a priority list of subtitle languages to use. Different container formats employ different language codes. DVDs use ISO 639-1 two letter language codes, Matroska uses ISO 639-2 three letter language codes while OGM uses a free-form identifier. See also --sid.
        /// </summary>
        public MpvOptionList SubLanguage => _subLanguage ??= new MpvOptionList(this, "slang");
        private MpvOptionList? _subLanguage;
        /// <summary>
        /// Specify a priority list of video languages to use. Different container formats employ different language codes. DVDs use ISO 639-1 two-letter language codes, Matroska, MPEG-TS and NUT use ISO 639-2 three-letter language codes, while OGM uses a free-form identifier. See also --vid.
        /// </summary>
        public MpvOptionList VideoLanguage => _videoLanguage ??= new MpvOptionList(this, "vlang");
        private MpvOptionList? _videoLanguage;
        /// <summary>
        /// Selects audio channel specified by <ID>. 'auto' selects the default, 'no' disables audio.
        /// </summary>
        public MpvOptionAutoNo<int?> AudioId => _audioId ??= new MpvOptionAutoNo<int?>(this, "aid");
        private MpvOptionAutoNo<int?>? _audioId;
        /// <summary>
        /// Display the subtitle stream specified by <ID>. 'auto' selects the default, 'no' disables subtitles.
        /// </summary>
        public MpvOptionAutoNo<int?> SubId => _subId ??= new MpvOptionAutoNo<int?>(this, "sid");
        private MpvOptionAutoNo<int?>? _subId;
        /// <summary>
        /// Select video channel specified by <ID>. 'auto' selects the default, 'no' disables video.
        /// </summary>
        public MpvOptionAutoNo<int?> VideoId => _videoId ??= new MpvOptionAutoNo<int?>(this, "vid");
        private MpvOptionAutoNo<int?>? _videoId;
        /// <summary>
        /// (Matroska files only) Specify the edition (set of chapters) to use, where 0 is the first. If set to 'auto' (the default), mpv will choose the first edition declared as a default, or if there is no default, the first edition defined.
        /// </summary>
        public MpvOptionAuto<int?> Edition => _edition ??= new MpvOptionAuto<int?>(this, "edition");
        private MpvOptionAuto<int?>? _edition;
        /// <summary>
        /// Enable the default track auto-selection (default: yes). Enabling this will make the player select streams according to --aid, --alang, and others. If it is disabled, no tracks are selected. In addition, the player will not exit if no tracks are selected, and wait instead (this wait mode is similar to pausing, but the pause option is not set).
        /// </summary>
        public MpvOption<bool?> TrackAutoSelection => _trackAutoSelection ??= new MpvOption<bool?>(this, "track-auto-selection");
        private MpvOption<bool?>? _trackAutoSelection;

        ///// <summary>
        ///// Specify a list of audio filters to apply to the audio stream. See AUDIO FILTERS for details and descriptions of the available filters.
        ///// </summary>
        //public MpvOptionList AudioFilter => _audioFilter ??= new MpvOptionList(this, "af");
        //private MpvOptionList? _audioFilter;
        ///// <summary>
        ///// Specify a list of video filters to apply to the video stream. See VIDEO FILTERS for details and descriptions of the available filters.
        ///// </summary>
        //public MpvOptionList VideoFilter => _videoFilter ??= new MpvOptionList(this, "vf");
        //private MpvOptionList? _videoFilter;

        /// <summary>
        /// Seek to given time position.
        /// The general format for times is [+|-] [[hh:] mm:]ss[.ms]. If the time is prefixed with -, the time is considered relative from the end of the file(as signaled by the demuxer/the file). A + is usually ignored(but see below).
        /// The following alternative time specifications are recognized:
        /// pp% seeks to percent position pp(0-100).
        /// #c seeks to chapter number c. (Chapters start from 1.)
        /// none resets any previously set option(useful for libmpv).
        /// </summary>
        public MpvOption<string?> Start => _start ??= new MpvOption<string?>(this, "start");
        private MpvOption<string?>? _start;
        /// <summary>
        /// Stop at given time. Use --length if the time should be relative to --start. See --start for valid option values and examples.
        /// </summary>
        public MpvOption<string?> End => _end ??= new MpvOption<string?>(this, "end");
        private MpvOption<string?>? _end;
        /// <summary>
        /// Stop after a given time relative to the start time. See --start for valid option values and examples.
        /// If both --end and --length are provided, playback will stop when it reaches either of the two endpoints.
        /// Obscurity note: this does not work correctly if --rebase-start-time=no, and the specified time is not an "absolute" time, as defined in the --start option description.
        /// </summary>
        public MpvOption<string?> Length => _length ??= new MpvOption<string?>(this, "length");
        private MpvOption<string?>? _length;
        /// <summary>
        /// Whether to move the file start time to 00:00:00 (default: yes). This is less awkward for files which start at a random timestamp, such as transport streams. On the other hand, if there are timestamp resets, the resulting behavior can be rather weird. For this reason, and in case you are actually interested in the real timestamps, this behavior can be disabled with no.
        /// </summary>
        public MpvOption<bool?> RebaseStartTime => _rebaseStartTime ??= new MpvOption<bool?>(this, "rebase-start-time");
        private MpvOption<bool?>? _rebaseStartTime;
        /// <summary>
        /// Slow down or speed up playback by the factor given as parameter.
        /// If --audio-pitch-correction(on by default) is used, playing with a speed higher than normal automatically inserts the scaletempo audio filter.
        /// </summary>
        public MpvOption<float?> Speed => _speed ??= new MpvOption<float?>(this, "speed");
        private MpvOption<float?>? _speed;
        /// <summary>
        /// Whether the player is in a paused state.
        /// </summary>
        public MpvOption<bool?> Pause => _pause ??= new MpvOption<bool?>(this, "pause");
        private MpvOption<bool?>? _pause;
        /// <summary>
        /// Whether to play files in a random order.
        /// </summary>
        public MpvOption<bool?> Shuffle => _shuffle ??= new MpvOption<bool?>(this, "shuffle");
        private MpvOption<bool?>? _shuffle;
        /// <summary>
        /// Set which file on the internal playlist to start playback with. The index is an integer, with 0 meaning the first file. The value auto means that the selection of the entry to play is left to the playback resume mechanism (default). If an entry with the given index doesn't exist, the behavior is unspecified and might change in future mpv versions. The same applies if the playlist contains further playlists (don't expect any reasonable behavior). Passing a playlist file to mpv should work with this option, though. E.g. mpv playlist.m3u --playlist-start=123 will work as expected, as long as playlist.m3u does not link to further playlists.
        /// </summary>
        public MpvOptionAuto<int?> PlaylistStart => _playlistStart ??= new MpvOptionAuto<int?>(this, "playlist-start");
        private MpvOptionAuto<int?>? _playlistStart;

    }
}
