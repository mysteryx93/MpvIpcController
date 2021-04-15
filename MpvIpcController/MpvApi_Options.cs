using System;

namespace HanumanInstitute.MpvIpcController
{
    public partial class MpvApi
    {
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
        /// If --audio-pitch-correction(on by default) is used, playing with a speed higher than normal automatically inserts the scaletempo2 audio filter.
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
        /// <summary>
        /// Play files according to a playlist file (Supports some common formats. If no format is detected, it will be treated as list of files, separated by newline characters. Note that XML playlist formats are not supported.)
        /// You can play playlists directly and without this option, however, this option disables any security mechanisms that might be in place.You may also need this option to load plaintext files as playlist.
        /// Do NOT use Playlist with random internet sources or files you do not trust!
        /// </summary>
        public MpvOption<string?> Playlist => _playlist ??= new MpvOption<string?>(this, "playlist");
        private MpvOption<string?>? _playlist;
        /// <summary>
        /// Threshold for merging almost consecutive ordered chapter parts in milliseconds (default: 100). Some Matroska files with ordered chapters have inaccurate chapter end timestamps, causing a small gap between the end of one chapter and the start of the next one when they should match. If the end of one playback part is less than the given threshold away from the start of the next one then keep playing video normally over the chapter change instead of doing a seek.
        /// </summary>
        public MpvOption<int?> ChapterMergeThreshold => _chapterMergeThreshold ??= new MpvOption<int?>(this, "chapter-merge-threshold");
        private MpvOption<int?>? _chapterMergeThreshold;
        /// <summary>
        /// Distance in seconds from the beginning of a chapter within which a backward chapter seek will go to the previous chapter (default: 5.0). Past this threshold, a backward chapter seek will go to the beginning of the current chapter instead. A negative value means always go back to the previous chapter.
        /// </summary>
        public MpvOption<double?> ChapterSeekThreshold => _chapterSeekThreshold ??= new MpvOption<double?>(this, "chapter-seek-threshold");
        private MpvOption<double?>? _chapterSeekThreshold;
        /// <summary>
        /// Select when to use precise seeks that are not limited to keyframes. Such seeks require decoding video from the previous keyframe up to the target position and so can take some time depending on decoding performance. For some video formats, precise seeks are disabled. This option selects the default choice to use for seeks; it is possible to explicitly override that default in the definition of key bindings and in input commands.
        /// </summary>
        public MpvOptionEnum<HrSeekOption> HrSeek => _hrSeek ??= new MpvOptionEnum<HrSeekOption>(this, "hr-seek");
        private MpvOptionEnum<HrSeekOption>? _hrSeek;
        /// <summary>
        /// This option exists to work around failures to do precise seeks (as in --hr-seek) caused by bugs or limitations in the demuxers for some file formats. Some demuxers fail to seek to a keyframe before the given target position, going to a later position instead. The value of this option is subtracted from the time stamp given to the demuxer. Thus, if you set this option to 1.5 and try to do a precise seek to 60 seconds, the demuxer is told to seek to time 58.5, which hopefully reduces the chance that it erroneously goes to some time later than 60 seconds. The downside of setting this option is that precise seeks become slower, as video between the earlier demuxer position and the real target may be unnecessarily decoded.
        /// </summary>
        public MpvOption<double?> HrSeekDemuxerOffset => _hrSeekDemuxerOffset ??= new MpvOption<double?>(this, "hr-seek-demuxer-offset");
        private MpvOption<double?>? _hrSeekDemuxerOffset;
        /// <summary>
        /// Allow the video decoder to drop frames during seek, if these frames are before the seek target. If this is enabled, precise seeking can be faster, but if you're using video filters which modify timestamps or add new frames, it can lead to precise seeking skipping the target frame. This e.g. can break frame backstepping when deinterlacing is enabled.
        /// </summary>
        public MpvOption<bool?> HrSeekFrameDrop => _hrSeekFrameDrop ??= new MpvOption<bool?>(this, "hr-seek-framedrop");
        private MpvOption<bool?>? _hrSeekFrameDrop;
        /// <summary>
        /// Controls how to seek in files. Note that if the index is missing from a file, it will be built on the fly by default, so you don't need to change this. But it might help with some broken files.
        /// This option only works if the underlying media supports seeking (i.e. not with stdin, pipe, etc).
        /// </summary>
        public MpvOptionEnum<IndexMode> Index => _index ??= new MpvOptionEnum<IndexMode>(this, "index");
        private MpvOptionEnum<IndexMode>? _index;
        /// <summary>
        /// Load URLs from playlists which are considered unsafe (default: no). This includes special protocols and anything that doesn't refer to normal files. Local files and HTTP links on the other hand are always considered safe.
        /// In addition, if a playlist is loaded while this is set, the added playlist entries are not marked as originating from network or potentially unsafe location. (Instead, the behavior is as if the playlist entries were provided directly to mpv command line or loadfile command.)
        /// </summary>
        public MpvOption<bool?> LoadUnsafePlaylists => _loadUnsafePlaylists ??= new MpvOption<bool?>(this, "load-unsafe-playlists");
        private MpvOption<bool?>? _loadUnsafePlaylists;
        /// <summary>
        /// Follow any references in the file being opened (default: yes). Disabling this is helpful if the file is automatically scanned (e.g. thumbnail generation). If the thumbnail scanner for example encounters a playlist file, which contains network URLs, and the scanner should not open these, enabling this option will prevent it. This option also disables ordered chapters, mov reference files, opening of archives, and a number of other features.
        /// </summary>
        public MpvOption<bool?> AccessReferences => _accessReferences ??= new MpvOption<bool?>(this, "access-references");
        private MpvOption<bool?>? _accessReferences;
        /// <summary>
        /// Loops playback N times. A value of 1 plays it one time (default), 2 two times, etc. inf means forever. no is the same as 1 and disables looping. If several files are specified on command line, the entire playlist is looped. --loop-playlist is the same as --loop-playlist=inf.
        /// The force mode is like inf, but does not skip playlist entries which have been marked as failing.This means the player might waste CPU time trying to loop a file that doesn't exist. But it might be useful for playing webradios under very bad network conditions.
        /// </summary>
        public MpvOption<string?> LoopPlaylist => _loopPlaylist ??= new MpvOption<string?>(this, "loop-playlist");
        private MpvOption<string?>? _loopPlaylist;
        /// <summary>
        /// Loop a single file N times. inf means forever, no means normal playback. For compatibility, --loop-file and --loop-file=yes are also accepted, and are the same as --loop-file=inf.
        /// The difference to --loop-playlist is that this doesn't loop the playlist, just the file itself. If the playlist contains only a single file, the difference between the two option is that this option performs a seek on loop, instead of reloading the file.
        /// </summary>
        public MpvOption<string?> LoopFile => _loopFile ??= new MpvOption<string?>(this, "loop-file");
        private MpvOption<string?>? _loopFile;
        /// <summary>
        /// Set loop points. If playback passes the b timestamp, it will seek to the a timestamp. Seeking past the b point doesn't loop (this is intentional). If either options are set to no (or unset), looping is disabled.
        /// </summary>
        public MpvOption<string?> AbLoopA => _abLoopA ??= new MpvOption<string?>(this, "ab-loop-a");
        private MpvOption<string?>? _abLoopA;
        /// <summary>
        /// Set loop points. If playback passes the b timestamp, it will seek to the a timestamp. Seeking past the b point doesn't loop (this is intentional). If either options are set to no (or unset), looping is disabled.
        /// </summary>
        public MpvOption<string?> AbLoopB => _abLoopB ??= new MpvOption<string?>(this, "ab-loop-b");
        private MpvOption<string?>? _abLoopB;
        /// <summary>
        /// Enabled by default. Whether to use Matroska ordered chapters. mpv will not load or search for video segments from other files, and will also ignore any chapter order specified for the main file.
        /// </summary>
        public MpvOption<bool?> OrderedChapters => _orderedChapters ??= new MpvOption<bool?>(this, "ordered-chapters");
        private MpvOption<bool?>? _orderedChapters;
    }
}
