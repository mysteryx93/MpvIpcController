using System;
using System.Collections.Generic;

// MPV JSON IPC protocol documentation
// https://mpv.io/manual/stable/#json-ipc
//
// Based on v0.32
//
// Changelog to update for future versions available here
// https://github.com/mpv-player/mpv/releases
// https://github.com/mpv-player/mpv/blob/master/DOCS/client-api-changes.rst

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Exposes MPV's API in a strongly-typed way.
    /// </summary>
    public partial class MpvApi : IDisposable
    {
        /// <summary>
        /// Factor multiplied with speed at which the player attempts to play the file. Usually it's exactly 1. (Display sync mode will make this useful.)
        /// </summary>
        public MpvPropertyRead<float?> AudioSpeedCorrection => _audioSpeedCorrection ??= new MpvPropertyRead<float?>(this, "audio-speed-correction");
        private MpvPropertyRead<float?>? _audioSpeedCorrection;

        /// <summary>
        /// Factor multiplied with speed at which the player attempts to play the file. Usually it's exactly 1. (Display sync mode will make this useful.)
        /// </summary>
        public MpvPropertyRead<float?> VideoSpeedCorrection => _videoSpeedCorrection ??= new MpvPropertyRead<float?>(this, "video-speed-correction");
        private MpvPropertyRead<float?>? _videoSpeedCorrection;

        /// <summary>
        /// Return whether --video-sync=display is actually active.
        /// </summary>
        public MpvPropertyRead<bool?> DisplaySyncActive => _displaySyncActive ??= new MpvPropertyRead<bool?>(this, "display-sync-active");
        private MpvPropertyRead<bool?>? _displaySyncActive;

        /// <summary>
        /// Currently played file, with path stripped. If this is an URL, try to undo percent encoding as well. (The result is not necessarily correct, but looks better for display purposes. Use the path property to get an unmodified filename.)
        /// </summary>
        public MpvPropertyRead<string?> FileName => _fileName ??= new MpvPropertyRead<string?>(this, "filename");
        private MpvPropertyRead<string?>? _fileName;

        /// <summary>
        /// Like the filename property, but if the text contains a ., strip all text after the last .. Usually this removes the file extension.
        /// </summary>
        public MpvPropertyRead<string?> FileNameNoExt => _fileNameNoExt ??= new MpvPropertyRead<string?>(this, "filename/no-ext");
        private MpvPropertyRead<string?>? _fileNameNoExt;

        /// <summary>
        /// Length in bytes of the source file/stream. (This is the same as ${stream-end}. For segmented/multi-part files, this will return the size of the main or manifest file, whatever it is.)
        /// </summary>
        public MpvPropertyRead<long?> FileSize => _fileSize ??= new MpvPropertyRead<long?>(this, "file-size");
        private MpvPropertyRead<long?>? _fileSize;

        /// <summary>
        /// Total number of frames in current file. This is only an estimate. (It's computed from two unreliable quantities: fps and stream length.)
        /// </summary>
        public MpvPropertyRead<long?> EstimatedFrameCount => _estimatedFrameCount ??= new MpvPropertyRead<long?>(this, "estimated-frame-count");
        private MpvPropertyRead<long?>? _estimatedFrameCount;

        /// <summary>
        /// Number of current frame in current stream.This is only an estimate. (It's computed from two unreliable quantities: fps and possibly rounded timestamps.)
        /// </summary>
        public MpvPropertyRead<long?> EstimatedFrameNumber => _estimatedFrameNumber ??= new MpvPropertyRead<long?>(this, "estimated-frame-number");
        private MpvPropertyRead<long?>? _estimatedFrameNumber;

        /// <summary>
        /// Full path of the currently played file. Usually this is exactly the same string you pass on the mpv command line or the loadfile command, even if it's a relative path. If you expect an absolute path, you will have to determine it yourself, for example by using the working-directory property.
        /// </summary>
        public MpvPropertyRead<string?> Path => _path ??= new MpvPropertyRead<string?>(this, "path");
        private MpvPropertyRead<string?>? _path;

        /// <summary>
        /// The full path to the currently played media. This is different only from path in special cases. In particular, if --ytdl=yes is used, and the URL is detected by youtube-dl, then the script will set this property to the actual media URL. This property should be set only during the on_load or on_load_fail hooks, otherwise it will have no effect (or may do something implementation defined in the future). The property is reset if playback of the current media ends.
        /// </summary>
        public MpvPropertyRead<string?> StreamOpenFileName => _streamOpenFileName ??= new MpvPropertyRead<string?>(this, "stream-open-filename");
        private MpvPropertyRead<string?>? _streamOpenFileName;

        /// <summary>
        /// If the currently played file has a title tag, use that. Otherwise, return the filename property.
        /// </summary>
        public MpvPropertyRead<string?> MediaTitle => _mediaTitle ??= new MpvPropertyRead<string?>(this, "media-title");
        private MpvPropertyRead<string?>? _mediaTitle;

        /// <summary>
        /// Symbolic name of the file format. In some cases, this is a comma-separated list of format names, e.g. mp4 is mov,mp4,m4a,3gp,3g2,mj2 (the list may grow in the future for any format).
        /// </summary>
        public MpvPropertyRead<string?> FileFormat => _fileFormat ??= new MpvPropertyRead<string?>(this, "file-format");
        private MpvPropertyRead<string?>? _fileFormat;

        /// <summary>
        /// Filename (full path) of the stream layer filename. (This is probably useless and is almost never different from path.)
        /// </summary>
        public MpvPropertyRead<string?> StreamPath => _streamPath ??= new MpvPropertyRead<string?>(this, "stream-path");
        private MpvPropertyRead<string?>? _streamPath;

        /// <summary>
        /// Raw byte position in source stream. Technically, this returns the position of the most recent packet passed to a decoder.
        /// </summary>
        public MpvPropertyRead<long?> StreamPos => _streamPos ??= new MpvPropertyRead<long?>(this, "stream-pos");
        private MpvPropertyRead<long?>? _streamPos;

        /// <summary>
        /// Raw end position in bytes in source stream.
        /// </summary>
        public MpvPropertyRead<long?> StreamEnd => _streamEnd ??= new MpvPropertyRead<long?>(this, "stream-end");
        private MpvPropertyRead<long?>? _streamEnd;

        /// <summary>
        /// Duration of the current file in seconds. If the duration is unknown, the property is unavailable. Note that the file duration is not always exactly known, so this is an estimate.
        /// </summary>
        public MpvPropertyRead<double?> Duration => _duration ??= new MpvPropertyRead<double?>(this, "duration");
        private MpvPropertyRead<double?>? _duration;

        /// <summary>
        /// Last A/V synchronization difference. Unavailable if audio or video is disabled.
        /// </summary>
        public MpvPropertyRead<float?> AVSync => _avSync ??= new MpvPropertyRead<float?>(this, "avsync");
        private MpvPropertyRead<float?>? _avSync;

        /// <summary>
        /// Total A-V sync correction done. Unavailable if audio or video is disabled.
        /// </summary>
        public MpvPropertyRead<float?> TotalAVSyncChange => _totalAVSyncChange ??= new MpvPropertyRead<float?>(this, "total-avsync-change");
        private MpvPropertyRead<float?>? _totalAVSyncChange;

        /// <summary>
        /// Video frames dropped by decoder, because video is too far behind audio (when using --framedrop=decoder). Sometimes, this may be incremented in other situations, e.g. when video packets are damaged, or the decoder doesn't follow the usual rules. Unavailable if video is disabled.
        /// </summary>
        public MpvPropertyRead<long?> DecoderFrameDropCount => _decoderFrameDropCount ??= new MpvPropertyRead<long?>(this, "decoder-frame-drop-count");
        private MpvPropertyRead<long?>? _decoderFrameDropCount;

        /// <summary>
        /// Frames dropped by VO (when using --framedrop=vo).
        /// </summary>
        public MpvPropertyRead<long?> FrameDropCount => _frameDropCount ??= new MpvPropertyRead<long?>(this, "frame-drop-count");
        private MpvPropertyRead<long?>? _frameDropCount;

        /// <summary>
        /// Number of video frames that were not timed correctly in display-sync mode for the sake of keeping A/V sync. This does not include external circumstances, such as video rendering being too slow or the graphics driver somehow skipping a vsync. It does not include rounding errors either (which can happen especially with bad source timestamps). For example, using the display-desync mode should never change this value from 0.
        /// </summary>
        public MpvPropertyRead<long?> MistimedFrameCount => _mistimedFrameCount ??= new MpvPropertyRead<long?>(this, "mistimed-frame-count");
        private MpvPropertyRead<long?>? _mistimedFrameCount;

        /// <summary>
        /// For how many vsyncs a frame is displayed on average. This is available if display-sync is active only. For 30 FPS video on a 60 Hz screen, this will be 2. This is the moving average of what actually has been scheduled, so 24 FPS on 60 Hz will never remain exactly on 2.5, but jitter depending on the last frame displayed.
        /// </summary>
        public MpvPropertyRead<float?> VSyncRatio => _vSyncRatio ??= new MpvPropertyRead<float?>(this, "vsync-ratio");
        private MpvPropertyRead<float?>? _vSyncRatio;

        /// <summary>
        /// Estimated number of frames delayed due to external circumstances in display-sync mode. Note that in general, mpv has to guess that this is happening, and the guess can be inaccurate.
        /// </summary>
        public MpvPropertyRead<long?> VoDelayedFrameCount => _voDelayedFrameCount ??= new MpvPropertyRead<long?>(this, "vo-delayed-frame-count");
        private MpvPropertyRead<long?>? _voDelayedFrameCount;

        /// <summary>
        /// Position in current file (0-100). The advantage over using this instead of calculating it out of other properties is that it properly falls back to estimating the playback position from the byte position, if the file duration is not known.
        /// </summary>
        public MpvPropertyWrite<float?> PercentPos => _percentPos ??= new MpvPropertyWrite<float?>(this, "percent-pos");
        private MpvPropertyWrite<float?>? _percentPos;

        /// <summary>
        /// Position in current file in seconds.
        /// </summary>
        public MpvPropertyWrite<double?> TimePos => _timePos ??= new MpvPropertyWrite<double?>(this, "time-pos");
        private MpvPropertyWrite<double?>? _timePos;

        /// <summary>
        /// Remaining length of the file in seconds. Note that the file duration is not always exactly known, so this is an estimate.
        /// </summary>
        public MpvPropertyRead<double?> TimeRemaining => _timeRemaining ??= new MpvPropertyRead<double?>(this, "time-remaining");
        private MpvPropertyRead<double?>? _timeRemaining;

        /// <summary>
        /// Current audio playback position in current file in seconds. Unlike time-pos, this updates more often than once per frame. For audio-only files, it is mostly equivalent to time-pos, while for video-only files this property is not available.
        /// </summary>
        public MpvPropertyRead<double?> AudioPts => _audioPts ??= new MpvPropertyRead<double?>(this, "audio-pts");
        private MpvPropertyRead<double?>? _audioPts;

        /// <summary>
        /// TimeRemaining scaled by the current speed.
        /// </summary>
        public MpvPropertyRead<double?> PlaytimeRemaining => _playtimeRemaining ??= new MpvPropertyRead<double?>(this, "playtime-remaining");
        private MpvPropertyRead<double?>? _playtimeRemaining;

        /// <summary>
        /// Position in current file in seconds. Unlike time-pos, the time is clamped to the range of the file. (Inaccurate file durations etc. could make it go out of range. Useful on attempts to seek outside of the file, as the seek target time is considered the current position during seeking.)
        /// </summary>
        public MpvPropertyWrite<double?> PlaybackTime => _playbackTime ??= new MpvPropertyWrite<double?>(this, "playback-time");
        private MpvPropertyWrite<double?>? _playbackTime;

        /// <summary>
        /// Current chapter number. The number of the first chapter is 0.
        /// </summary>
        public MpvPropertyWrite<int?> Chapter => _chapter ??= new MpvPropertyWrite<int?>(this, "chapter ");
        private MpvPropertyWrite<int?>? _chapter;

        /// <summary>
        /// Currently selected edition. This property is unavailable if no file is loaded, or the file has no editions. (Matroska files make a difference between having no editions and a single edition, which will be reflected by the property, although in practice it does not matter.)
        /// </summary>
        public MpvPropertyRead<int?> CurrentEdition => _currentEdition ??= new MpvPropertyRead<int?>(this, "current-edition");
        private MpvPropertyRead<int?>? _currentEdition;

        /// <summary>
        /// Number of chapters.
        /// </summary>
        public MpvPropertyRead<int?> Chapters => _chapters ??= new MpvPropertyRead<int?>(this, "chapters");
        private MpvPropertyRead<int?>? _chapters;

        /// <summary>
        /// Number of MKV editions.
        /// </summary>
        public MpvPropertyRead<int?> Editions => _editions ??= new MpvPropertyRead<int?>(this, "editions");
        private MpvPropertyRead<int?>? _editions;

        /// <summary>
        /// Number of editions. If there are no editions, this can be 0 or 1 (1 if there's a useless dummy edition).
        /// </summary>
        public MpvPropertyRead<int?> EditionListCount => _editionListCount ??= new MpvPropertyRead<int?>(this, "edition-list/count");
        private MpvPropertyRead<int?>? _editionListCount;

        /// <summary>
        /// Edition ID as integer. Use this to set the edition property. Currently, this is the same as the edition index.
        /// </summary>
        public MpvPropertyIndexWrite<int, int?> EditionListId => _editionListId ??= new MpvPropertyIndexWrite<int, int?>(this, "edition-list/{0}/id");
        private MpvPropertyIndexWrite<int, int?>? _editionListId;

        /// <summary>
        /// True if this is the default edition, otherwise false.
        /// </summary>
        public MpvPropertyIndexRead<int, bool?> EditionListDefault => _editionListDefault ??= new MpvPropertyIndexRead<int, bool?>(this, "edition-list/{0}/default");
        private MpvPropertyIndexRead<int, bool?>? _editionListDefault;

        /// <summary>
        /// Edition title as stored in the file. Not always available.
        /// </summary>
        public MpvPropertyIndexRead<int, string?> EditionListTitle => _editionListTitle ??= new MpvPropertyIndexRead<int, string?>(this, "edition-list/{0}/title");
        private MpvPropertyIndexRead<int, string?>? _editionListTitle;

        /// <summary>
        /// Metadata key/value pairs.
        /// </summary>
        public MetadataProperties Metadata => _metadata ??= new MetadataProperties(this, "metadata");
        private MetadataProperties? _metadata;

        /// <summary>
        /// Like metadata, but includes only fields listed in the --display-tags option. This is the same set of tags that is printed to the terminal.
        /// </summary>
        public MetadataProperties FilteredMetadata => _filteredMetadata ??= new MetadataProperties(this, "filtered-metadata");
        private MetadataProperties? _filteredMetadata;

        /// <summary>
        /// Metadata of current chapter. Works similar to metadata property. It also allows the same access methods (using sub-properties).
        /// Per-chapter metadata is very rare.Usually, only the chapter name (title) is set.
        /// For accessing other information, like chapter start, see the chapter-list property.
        /// </summary>
        public MetadataProperties ChapterMetadata => _chapterMetadata ??= new MetadataProperties(this, "chapter-metadata");
        private MetadataProperties? _chapterMetadata;

        /// <summary>
        /// Metadata added by video filters. Accessed by the filter label, which, if not explicitly specified using the @filter-label: syntax, will be <filter-name>NN.
        /// </summary>
        /// <param name="filterLabel">The label of the filter.</param>
        public MetadataProperties VideoFilterMetadata(string filterLabel) => new MetadataProperties(this, $"vf-metadata/{filterLabel}");

        /// <summary>
        /// Metadata added by audio filters. Accessed by the filter label, which, if not explicitly specified using the @filter-label: syntax, will be <filter-name>NN.
        /// </summary>
        /// <param name="filterLabel">The label of the filter.</param>
        public MetadataProperties AudioFilterMetadata(string filterLabel) => new MetadataProperties(this, $"af-metadata/{filterLabel}");

        /// <summary>
        /// Return yes if no file is loaded, but the player is staying around because of the --idle option.
        /// </summary>
        public MpvPropertyRead<bool?> IdleActive => _idleActive ??= new MpvPropertyRead<bool?>(this, "idle-active");
        private MpvPropertyRead<bool?>? _idleActive;

        /// <summary>
        /// Return yes if the playback core is paused, otherwise no. This can be different pause in special situations, such as when the player pauses itself due to low network cache.
        /// This also returns yes if playback is restarting or if nothing is playing at all.In other words, it's only no if there's actually video playing. (Behavior since mpv 0.7.0.)
        /// </summary>
        public MpvPropertyRead<bool?> CoreIdle => _coreIdle ??= new MpvPropertyRead<bool?>(this, "core-idle");
        private MpvPropertyRead<bool?>? _coreIdle;

        /// <summary>
        /// Current I/O read speed between the cache and the lower layer (like network). This gives the number bytes per seconds over a 1 second window.
        /// </summary>
        public MpvPropertyRead<long?> CacheSpeed => _cacheSpeed ??= new MpvPropertyRead<long?>(this, "cache-speed");
        private MpvPropertyRead<long?>? _cacheSpeed;

        /// <summary>
        /// Approximate duration of video buffered in the demuxer, in seconds. The guess is very unreliable, and often the property will not be available at all, even if data is buffered
        /// </summary>
        public MpvPropertyRead<double?> DemuxerCacheDuration => _demuxerCacheDuration ??= new MpvPropertyRead<double?>(this, "demuxer-cache-duration");
        private MpvPropertyRead<double?>? _demuxerCacheDuration;

        /// <summary>
        /// Approximate time of video buffered in the demuxer, in seconds. Same as demuxer-cache-duration but returns the last timestamp of buffered data in demuxer.
        /// </summary>
        public MpvPropertyRead<double?> DemuxerCacheTime => _demuxerCacheTime ??= new MpvPropertyRead<double?>(this, "demuxer-cache-time");
        private MpvPropertyRead<double?>? _demuxerCacheTime;

        /// <summary>
        /// Returns yes if the demuxer is idle, which means the demuxer cache is filled to the requested amount, and is currently not reading more data.
        /// </summary>
        public MpvPropertyRead<bool?> DemuxerCacheIdle => _demuxerCacheIdle ??= new MpvPropertyRead<bool?>(this, "demuxer-cache-idle");
        private MpvPropertyRead<bool?>? _demuxerCacheIdle;

        /// <summary>
        /// Various undocumented or half-documented things.
        /// </summary>
        public MpvPropertyRead<DemuxerCacheState?> DemuxerCacheState => _demuxerCacheState ??= new MpvPropertyRead<DemuxerCacheState?>(this, "demuxer-cache-state");
        private MpvPropertyRead<DemuxerCacheState?>? _demuxerCacheState;

        /// <summary>
        /// Returns true if the stream demuxed via the main demuxer is most likely played via network. What constitutes "network" is not always clear, might be used for other types of untrusted streams, could be wrong in certain cases, and its definition might be changing. Also, external files (like separate audio files or streams) do not influence the value of this property (currently).
        /// </summary>
        public MpvPropertyRead<bool?> DemuxerViaNetwork => _demuxerViaNetwork ??= new MpvPropertyRead<bool?>(this, "demuxer-via-network");
        private MpvPropertyRead<bool?>? _demuxerViaNetwork;

        /// <summary>
        /// Returns the start time reported by the demuxer in fractional seconds.
        /// </summary>
        public MpvPropertyRead<double?> DemuxerStartTime => _demuxerStartTime ??= new MpvPropertyRead<double?>(this, "demuxer-start-time");
        private MpvPropertyRead<double?>? _demuxerStartTime;

        /// <summary>
        /// Returns True when playback is paused because of waiting for the cache.
        /// </summary>
        public MpvPropertyRead<bool?> PausedForCache => _pausedForCache ??= new MpvPropertyRead<bool?>(this, "paused-for-cache");
        private MpvPropertyRead<bool?>? _pausedForCache;

        /// <summary>
        /// Returns the percentage (0-100) of the cache fill status until the player will unpause (related to paused-for-cache).
        /// </summary>
        public MpvPropertyRead<double?> CacheBufferingState => _cacheBufferingState ??= new MpvPropertyRead<double?>(this, "cache-buffering-state");
        private MpvPropertyRead<double?>? _cacheBufferingState;

        /// <summary>
        /// Returns true if end of playback was reached, no otherwise. Note that this is usually interesting only if --keep-open is enabled, since otherwise the player will immediately play the next file (or exit or enter idle mode), and in these cases the eof-reached property will logically be cleared immediately after it's set.
        /// </summary>
        public MpvPropertyRead<bool?> EofReached => _eofReached ??= new MpvPropertyRead<bool?>(this, "eof-reached");
        private MpvPropertyRead<bool?>? _eofReached;

        /// <summary>
        /// Returns True if the player is currently seeking, or otherwise trying to restart playback. (It's possible that it returns True while a file is loaded. This is because the same underlying code is used for seeking and resyncing.)
        /// </summary>
        public MpvPropertyRead<bool?> Seeking => _seeking ??= new MpvPropertyRead<bool?>(this, "seeking");
        private MpvPropertyRead<bool?>? _seeking;

        /// <summary>
        /// Return yes if the audio mixer is active, no otherwise.
        /// This option is relatively useless.Before mpv 0.18.1, it could be used to infer behavior of the volume property.
        /// </summary>
        public MpvPropertyRead<bool?> MixerActive => _mixerActive ??= new MpvPropertyRead<bool?>(this, "mixer-active");
        private MpvPropertyRead<bool?>? _mixerActive;

        /// <summary>
        /// System volume. This property is available only if mpv audio output is currently active, and only if the underlying implementation supports volume control. What this option does depends on the API. For example, on ALSA this usually changes system-wide audio, while with PulseAudio this controls per-application volume.
        /// </summary>
        public MpvPropertyWrite<double?> AoVolume => _aoVolume ??= new MpvPropertyWrite<double?>(this, "ao-volume");
        private MpvPropertyWrite<double?>? _aoVolume;

        /// <summary>
        /// Similar to AoVolume, but controls the mute state. May be unimplemented even if AoVolume works.
        /// </summary>
        public MpvPropertyWrite<double?> AoMute => _aoMute ??= new MpvPropertyWrite<double?>(this, "ao-mute");
        private MpvPropertyWrite<double?>? _aoMute;

        /// <summary>
        /// Audio codec selected for decoding.
        /// </summary>
        public MpvPropertyRead<string?> AudioCodec => _audioCodec ??= new MpvPropertyRead<string?>(this, "audio-codec");
        private MpvPropertyRead<string?>? _audioCodec;

        /// <summary>
        /// Audio codec.
        /// </summary>
        public MpvPropertyRead<string?> AudioCodecName => _audioCodecName ??= new MpvPropertyRead<string?>(this, "audio-codec-name");
        private MpvPropertyRead<string?>? _audioCodecName;

        /// <summary>
        /// Audio format as output by the audio decoder. This has a number of sub-properties.
        /// </summary>
        public AudioProperties AudioParams => _audioParams ??= new AudioProperties(this, "audio-params");
        private AudioProperties? _audioParams;

        /// <summary>
        /// Audio format as output by the audio decoder. This has a number of sub-properties.
        /// </summary>
        public AudioProperties AudioOutParams => _audioOutParams ??= new AudioProperties(this, "audio-out-params");
        private AudioProperties? _audioOutParams;

        /// <summary>
        /// Reflects the --hwdec option.
        /// Writing to it may change the currently used hardware decoder, if possible. (Internally, the player may reinitialize the decoder, and will perform a seek to refresh the video properly.) You can watch the other hwdec properties to see whether this was successful.
        /// </summary>
        public MpvPropertyRead<string?> Hwdec => _hwdec ??= new MpvPropertyRead<string?>(this, "hwdec");
        private MpvPropertyRead<string?>? _hwdec;

        /// <summary>
        /// Returns the current hardware decoding in use. If decoding is active, return one of the values used by the hwdec option/property. no indicates software decoding. If no decoder is loaded, the property is unavailable.
        /// </summary>
        public MpvPropertyRead<string?> HwdecCurrent => _hwdecCurrent ??= new MpvPropertyRead<string?>(this, "hwdec-current");
        private MpvPropertyRead<string?>? _hwdecCurrent;

        /// <summary>
        /// This returns the currently loaded hardware decoding/output interop driver. This is known only once the VO has opened (and possibly later). With some VOs (like gpu), this might be never known in advance, but only when the decoder attempted to create the hw decoder successfully. (Using --gpu-hwdec-interop can load it eagerly.) If there are multiple drivers loaded, they will be separated by ,.
        /// If no VO is active or no interop driver is known, this property is unavailable.
        /// This does not necessarily use the same values as hwdec.There can be multiple interop drivers for the same hardware decoder, depending on platform and VO.
        /// </summary>
        public MpvPropertyRead<string?> HwdecInterop => _hwdecInterop ??= new MpvPropertyRead<string?>(this, "hwdec-interop");
        private MpvPropertyRead<string?>? _hwdecInterop;

        /// <summary>
        /// Video format as string.
        /// </summary>
        public MpvPropertyRead<string?> VideoFormat => _videoFormat ??= new MpvPropertyRead<string?>(this, "video-format");
        private MpvPropertyRead<string?>? _videoFormat;

        /// <summary>
        /// Video codec selected for decoding.
        /// </summary>
        public MpvPropertyRead<string?> VideoCodec => _videoCodec ??= new MpvPropertyRead<string?>(this, "video-codec");
        private MpvPropertyRead<string?>? _videoCodec;

        /// <summary>
        /// Returns the width of the video as decoded, or if no video frame has been decoded yet, the (possibly incorrect) container indicated size.
        /// </summary>
        public MpvPropertyRead<int?> Width => _width ??= new MpvPropertyRead<int?>(this, "width");
        private MpvPropertyRead<int?>? _width;

        /// <summary>
        /// Returns the height of the video as decoded, or if no video frame has been decoded yet, the (possibly incorrect) container indicated size.
        /// </summary>
        public MpvPropertyRead<int?> Height => _height ??= new MpvPropertyRead<int?>(this, "height");
        private MpvPropertyRead<int?>? _height;

        /// <summary>
        /// Video parameters, as output by the decoder (with overrides like aspect etc. applied). This has a number of sub-properties:
        /// </summary>
        public VideoProperties VideoParams => _videoParams ??= new VideoProperties(this, "video-params");
        private VideoProperties? _videoParams;

        /// <summary>
        /// This is the video display width after filters and aspect scaling have been applied. The actual video window size can still be different from this, e.g. if the user resized the video window manually. This has the same values as VideoOutParams/DisplayWidth.
        /// </summary>
        public MpvPropertyRead<int?> DisplayWidth => _displayWidth ??= new MpvPropertyRead<int?>(this, "dwidth");
        private MpvPropertyRead<int?>? _displayWidth;

        /// <summary>
        /// This is the video display height after filters and aspect scaling have been applied. The actual video window size can still be different from this, e.g. if the user resized the video window manually. This has the same values as VideoOutParams/DisplayHeight.
        /// </summary>
        public MpvPropertyRead<int?> DisplayHeight => _displayHeight ??= new MpvPropertyRead<int?>(this, "dheight");
        private MpvPropertyRead<int?>? _displayHeight;

        /// <summary>
        /// Exactly like VideoParams, but no overrides applied.
        /// </summary>
        public VideoProperties VideoDecParams => _videoDecParams ??= new VideoProperties(this, "video-dec-params");
        private VideoProperties? _videoDecParams;

        /// <summary>
        /// Same as video-params, but after video filters have been applied. If there are no video filters in use, this will contain the same values as video-params. Note that this is still not necessarily what the video window uses, since the user can change the window size, and all real VOs do their own scaling independently from the filter chain.
        /// </summary>
        public VideoProperties VideoOutParams => _videoOutParams ??= new VideoProperties(this, "video-out-params");
        private VideoProperties? _videoOutParams;

        /// <summary>
        /// Approximate information of the current frame.
        /// </summary>
        public MpvPropertyRead<string?> VideoFramePictureType => _videoFramePictureType ??= new MpvPropertyRead<string?>(this, "video-frame-info/picture-type");
        private MpvPropertyRead<string?>? _videoFramePictureType;

        /// <summary>
        /// Approximate information of the current frame.
        /// </summary>
        public MpvPropertyRead<bool?> VideoFrameInterlaced => _videoFrameInterlaced ??= new MpvPropertyRead<bool?>(this, "video-frame-info/interlaced");
        private MpvPropertyRead<bool?>? _videoFrameInterlaced;

        /// <summary>
        /// Approximate information of the current frame.
        /// </summary>
        public MpvPropertyRead<bool?> VideoFrameTff => _videoFrameTff ??= new MpvPropertyRead<bool?>(this, "video-frame-info/tff");
        private MpvPropertyRead<bool?>? _videoFrameTff;

        /// <summary>
        /// Approximate information of the current frame.
        /// </summary>
        public MpvPropertyRead<bool?> VideoFrameRepeat => _videoFrameRepeat ??= new MpvPropertyRead<bool?>(this, "video-frame-info/repeat");
        private MpvPropertyRead<bool?>? _videoFrameRepeat;

        /// <summary>
        /// Container FPS. This can easily contain bogus values. For videos that use modern container formats or video codecs, this will often be incorrect.
        /// </summary>
        public MpvPropertyRead<float?> ContainerFps => _containerFps ??= new MpvPropertyRead<float?>(this, "container-fps");
        private MpvPropertyRead<float?>? _containerFps;

        /// <summary>
        /// Estimated/measured FPS of the video filter chain output. (If no filters are used, this corresponds to decoder output.) This uses the average of the 10 past frame durations to calculate the FPS. It will be inaccurate if frame-dropping is involved (such as when framedrop is explicitly enabled, or after precise seeking). Files with imprecise timestamps (such as Matroska) might lead to unstable results.
        /// </summary>
        public MpvPropertyRead<float?> EstimatedVideoFilterFps => _estimatedVideoFilterFps ??= new MpvPropertyRead<float?>(this, "estimated-vf-fps");
        private MpvPropertyRead<float?>? _estimatedVideoFilterFps;

        /// <summary>
        /// Window size multiplier. Setting this will resize the video window to the values contained in dwidth and dheight multiplied with the value set with this property. Setting 1 will resize to original video size (or to be exact, the size the video filters output). 2 will set the double size, 0.5 halves the size.
        /// </summary>
        public MpvPropertyWrite<float?> WindowScale => _windowScale ??= new MpvPropertyWrite<float?>(this, "window-scale");
        private MpvPropertyWrite<float?>? _windowScale;

        /// <summary>
        /// Names of the displays that the mpv window covers. On X11, these are the xrandr names (LVDS1, HDMI1, DP1, VGA1, etc.). On Windows, these are the GDI names (\.DISPLAY1, \.DISPLAY2, etc.) and the first display in the list will be the one that Windows considers associated with the window (as determined by the MonitorFromWindow API.) On macOS these are the Display Product Names as used in the System Information and only one display name is returned since a window can only be on one screen.
        /// </summary>
        public MpvPropertyRead<IList<string>?> DisplayNames => _displayNames ??= new MpvPropertyRead<IList<string>?>(this, "display-names");
        private MpvPropertyRead<IList<string>?>? _displayNames;

        /// <summary>
        /// The refresh rate of the current display. Currently, this is the lowest FPS of any display covered by the video, as retrieved by the underlying system APIs (e.g. xrandr on X11). It is not the measured FPS. It's not necessarily available on all platforms. Note that any of the listed facts may change any time without a warning.
        /// </summary>
        public MpvPropertyRead<float?> DisplayFps => _displayFps ??= new MpvPropertyRead<float?>(this, "display-fps");
        private MpvPropertyRead<float?>? _displayFps;

        /// <summary>
        /// Only available if display-sync mode (as selected by --video-sync) is active. Returns the actual rate at which display refreshes seem to occur, measured by system time.
        /// </summary>
        public MpvPropertyRead<float?> EstimatedDisplayFps => _estimatedDisplayFps ??= new MpvPropertyRead<float?>(this, "estimated-display-fps");
        private MpvPropertyRead<float?>? _estimatedDisplayFps;

        /// <summary>
        /// Estimated deviation factor of the vsync duration.
        /// </summary>
        public MpvPropertyRead<float?> VSyncJitter => _vSyncJitter ??= new MpvPropertyRead<float?>(this, "vsync-jitter");
        private MpvPropertyRead<float?>? _vSyncJitter;

        /// <summary>
        /// The HiDPI scale factor as reported by the windowing backend. If no VO is active, or if the VO does not report a value, this property is unavailable. It may be saner to report an absolute DPI, however, this is the way HiDPI support is implemented on most OS APIs. See also --hidpi-window-scale.
        /// </summary>
        public MpvPropertyRead<float?> DisplayHiDpiScale => _displayHiDpiScale ??= new MpvPropertyRead<float?>(this, "display-hidpi-scale");
        private MpvPropertyRead<float?>? _displayHiDpiScale;

        /// <summary>
        /// Width of the VO window in OSD render units (usually pixels, but may be scaled pixels with VOs like xv).
        /// </summary>
        public MpvPropertyRead<int?> OsdWidth => _osdWidth ??= new MpvPropertyRead<int?>(this, "osd-dimensions/w");
        private MpvPropertyRead<int?>? _osdWidth;

        /// <summary>
        /// Height of the VO window in OSD render units,
        /// </summary>
        public MpvPropertyRead<int?> OsdHeight => _osdHeight ??= new MpvPropertyRead<int?>(this, "osd-dimensions/h");
        private MpvPropertyRead<int?>? _osdHeight;

        /// <summary>
        /// Pixel aspect ratio of the OSD (usually 1).
        /// </summary>
        public MpvPropertyRead<float?> OsdPixelAspectRatio => _osdPixelAspectRatio ??= new MpvPropertyRead<float?>(this, "osd-dimensions/par");
        private MpvPropertyRead<float?>? _osdPixelAspectRatio;

        /// <summary>
        /// Display aspect ratio of the VO window. (Computing from the properties above.)
        /// </summary>
        public MpvPropertyRead<float?> OsdDisplayAspectRatio => _osdDisplayAspectRatio ??= new MpvPropertyRead<float?>(this, "osd-dimensions/aspect");
        private MpvPropertyRead<float?>? _osdDisplayAspectRatio;

        /// <summary>
        /// OSD to top video margins. This describes the area into which the video is rendered.
        /// </summary>
        public MpvPropertyRead<int?> OsdMarginTop => _osdMarginTop ??= new MpvPropertyRead<int?>(this, "osd-dimensions/mt");
        private MpvPropertyRead<int?>? _osdMarginTop;

        /// <summary>
        /// OSD to bottom video margins. This describes the area into which the video is rendered.
        /// </summary>
        public MpvPropertyRead<int?> OsdMarginBottom => _osdMarginBottom ??= new MpvPropertyRead<int?>(this, "osd-dimensions/mb");
        private MpvPropertyRead<int?>? _osdMarginBottom;

        /// <summary>
        /// OSD to left video margins. This describes the area into which the video is rendered.
        /// </summary>
        public MpvPropertyRead<int?> OsdMarginLeft => _osdMarginLeft ??= new MpvPropertyRead<int?>(this, "osd-dimensions/ml");
        private MpvPropertyRead<int?>? _osdMarginLeft;

        /// <summary>
        /// OSD to right video margins. This describes the area into which the video is rendered.
        /// </summary>
        public MpvPropertyRead<int?> OsdMarginRight => _osdMarginRight ??= new MpvPropertyRead<int?>(this, "osd-dimensions/mr");
        private MpvPropertyRead<int?>? _osdMarginRight;

        /// <summary>
        /// Return the current subtitle text regardless of sub visibility. Formatting is stripped. If the subtitle is not text-based (i.e. DVD/BD subtitles), an empty string is returned.
        /// This property is experimental and might be removed in the future.
        /// </summary>
        public MpvPropertyRead<string?> SubText => _subText ??= new MpvPropertyRead<string?>(this, "sub-text");
        private MpvPropertyRead<string?>? _subText;

        /// <summary>
        /// Returns the current subtitle start time (in seconds). If there's multiple current subtitles, returns the first start time. If no current subtitle is present null is returned instead.
        /// </summary>
        public MpvPropertyRead<float?> SubStart => _subStart ??= new MpvPropertyRead<float?>(this, "sub-start");
        private MpvPropertyRead<float?>? _subStart;

        /// <summary>
        /// Returns the current subtitle start time (in seconds). If there's multiple current subtitles, return the last end time. If no current subtitle is present, or if it's present but has unknown or incorrect duration, null is returned instead.
        /// </summary>
        public MpvPropertyRead<float?> SubEnd => _subEnd ??= new MpvPropertyRead<float?>(this, "sub-end");
        private MpvPropertyRead<float?>? _subEnd;

        /// <summary>
        /// Current position on playlist. The first entry is on position 0. Writing to the property will restart playback at the written entry.
        /// </summary>
        public MpvPropertyWrite<int?> PlaylistPosition => _playlistPosition ??= new MpvPropertyWrite<int?>(this, "playlist-pos");
        private MpvPropertyWrite<int?>? _playlistPosition;

        /// <summary>
        /// Number of total playlist entries.
        /// </summary>
        public MpvPropertyRead<int?> PlaylistCount => _playlistCount ??= new MpvPropertyRead<int?>(this, "playlist-count");
        private MpvPropertyRead<int?>? _playlistCount;

        /// <summary>
        /// Filename of the Nth playlist entry.
        /// </summary>
        public MpvPropertyIndexRead<int, string?> PlaylistFileName => _playlistFileName ??= new MpvPropertyIndexRead<int, string?>(this, "playlist/{0}/filename");
        private MpvPropertyIndexRead<int, string?>? _playlistFileName;

        /// <summary>
        /// True if this entry is currently playing (or being loaded). Unavailable or False otherwise. When changing files, current and playing can be different, because the currently playing file hasn't been unloaded yet; in this case, current refers to the new selection.
        /// </summary>
        public MpvPropertyIndexRead<int, bool?> PlaylistIsCurrent => _playlistIsCurrent ??= new MpvPropertyIndexRead<int, bool?>(this, "playlist/{0}/current");
        private MpvPropertyIndexRead<int, bool?>? _playlistIsCurrent;

        /// <summary>
        /// True if this entry is currently playing (or being loaded). Unavailable or False otherwise. When changing files, current and playing can be different, because the currently playing file hasn't been unloaded yet; in this case, current refers to the new selection.
        /// </summary>
        public MpvPropertyIndexRead<int, bool?> PlaylistIsPlaying => _playlistIsPlaying ??= new MpvPropertyIndexRead<int, bool?>(this, "playlist/{0}/playing");
        private MpvPropertyIndexRead<int, bool?>? _playlistIsPlaying;

        /// <summary>
        /// Name of the Nth entry. Only available if the playlist file contains such fields, and only if mpv's parser supports it for the given playlist format.
        /// </summary>
        public MpvPropertyIndexRead<int, string?> PlaylistTitle => _playlistTitle ??= new MpvPropertyIndexRead<int, string?>(this, "playlist/{0}/title");
        private MpvPropertyIndexRead<int, string?>? _playlistTitle;

        /// <summary>
        /// Total number of tracks.
        /// </summary>
        public MpvPropertyRead<int?> TrackListCount => _trackListCount ??= new MpvPropertyRead<int?>(this, "track-list/count");
        private MpvPropertyRead<int?>? _trackListCount;

        /// <summary>
        /// The ID as it's used for -sid/--aid/--vid. This is unique within tracks of the same type (sub/audio/video), but otherwise not.
        /// </summary>
        public MpvPropertyIndexRead<int, int?> TrackListId => _trackListId ??= new MpvPropertyIndexRead<int, int?>(this, "track-list/{0}/id");
        private MpvPropertyIndexRead<int, int?>? _trackListId;

        /// <summary>
        /// String describing the media type. One of audio, video, sub.
        /// </summary>
        public MpvPropertyIndexRead<int, string?> TrackListType => _trackListType ??= new MpvPropertyIndexRead<int, string?>(this, "track-list/{0}/type");
        private MpvPropertyIndexRead<int, string?>? _trackListType;

        /// <summary>
        /// Track ID as used in the source file. Not always available. (It is missing if the format has no native ID, if the track is a pseudo-track that does not exist in this way in the actual file, or if the format is handled by libavformat, and the format was not whitelisted as having track IDs.)
        /// </summary>
        public MpvPropertyIndexRead<int, int?> TrackListSrcId => _trackListSrcId ??= new MpvPropertyIndexRead<int, int?>(this, "track-list/{0}/src-id");
        private MpvPropertyIndexRead<int, int?>? _trackListSrcId;

        /// <summary>
        /// Track language as identified by the file. Not always available.
        /// </summary>
        public MpvPropertyIndexRead<int, string?> TrackListLanguage => _trackListLanguage ??= new MpvPropertyIndexRead<int, string?>(this, "track-list/{0}/lang");
        private MpvPropertyIndexRead<int, string?>? _trackListLanguage;

        /// <summary>
        /// True if this is a video track that consists of a single picture, False or unavailable otherwise. This is used for video tracks that are really attached pictures in audio files.
        /// </summary>
        public MpvPropertyIndexRead<int, bool?> TrackListHasAlbumArt => _trackListHasAlbumArt ??= new MpvPropertyIndexRead<int, bool?>(this, "track-list/{0}/albumart");
        private MpvPropertyIndexRead<int, bool?>? _trackListHasAlbumArt;

        /// <summary>
        /// True if the track has the default flag set in the file, otherwise False.
        /// </summary>
        public MpvPropertyIndexRead<int, bool?> TrackListIsDefault => _trackListIsDefault ??= new MpvPropertyIndexRead<int, bool?>(this, "track-list/{0}/default");
        private MpvPropertyIndexRead<int, bool?>? _trackListIsDefault;

        /// <summary>
        /// True if the track has the forced flag set in the file, otherwise False.
        /// </summary>
        public MpvPropertyIndexRead<int, bool?> TrackListIsForced => _trackListIsForced ??= new MpvPropertyIndexRead<int, bool?>(this, "track-list/{0}/forced");
        private MpvPropertyIndexRead<int, bool?>? _trackListIsForced;

        /// <summary>
        /// The codec name used by this track, for example h264. Unavailable in some rare cases.
        /// </summary>
        public MpvPropertyIndexRead<int, string?> TrackListCodec => _trackListCodec ??= new MpvPropertyIndexRead<int, string?>(this, "track-list/{0}/codec");
        private MpvPropertyIndexRead<int, string?>? _trackListCodec;

        /// <summary>
        /// True if the track is an external file, otherwise False. This is set for separate subtitle files.
        /// </summary>
        public MpvPropertyIndexRead<int, bool?> TrackListIsExternal => _trackListIsExternal ??= new MpvPropertyIndexRead<int, bool?>(this, "track-list/{0}/external");
        private MpvPropertyIndexRead<int, bool?>? _trackListIsExternal;

        /// <summary>
        /// The filename if the track is from an external file, unavailable otherwise.
        /// </summary>
        public MpvPropertyIndexRead<int, bool?> TrackListExternalFileName => _trackListExternalFileName ??= new MpvPropertyIndexRead<int, bool?>(this, "track-list/{0}/external-filename");
        private MpvPropertyIndexRead<int, bool?>? _trackListExternalFileName;

        /// <summary>
        /// True if the track is currently decoded, otherwise False.
        /// </summary>
        public MpvPropertyIndexRead<int, bool?> TrackListIsSelected => _trackListIsSelected ??= new MpvPropertyIndexRead<int, bool?>(this, "track-list/{0}/selected");
        private MpvPropertyIndexRead<int, bool?>? _trackListIsSelected;

        /// <summary>
        /// The stream index as usually used by the FFmpeg utilities. Note that this can be potentially wrong if a demuxer other than libavformat (--demuxer=lavf) is used. For mkv files, the index will usually match even if the default (builtin) demuxer is used, but there is no hard guarantee.
        /// </summary>
        public MpvPropertyIndexRead<int, int?> TrackListFfIndex => _trackListFfIndex ??= new MpvPropertyIndexRead<int, int?>(this, "track-list/{0}/ff-index");
        private MpvPropertyIndexRead<int, int?>? _trackListFfIndex;

        /// <summary>
        /// If this track is being decoded, the human-readable decoder name.
        /// </summary>
        public MpvPropertyIndexRead<int, string?> TrackListDecoderDesc => _trackListDecoderDesc ??= new MpvPropertyIndexRead<int, string?>(this, "track-list/{0}/decoder-desc");
        private MpvPropertyIndexRead<int, string?>? _trackListDecoderDesc;

        /// <summary>
        /// Video width hint as indicated by the container. (Not always accurate.)
        /// </summary>
        public MpvPropertyIndexRead<int, int?> TrackListDemuxWidth => _trackListDemuxWidth ??= new MpvPropertyIndexRead<int, int?>(this, "track-list/{0}/demux-w");
        private MpvPropertyIndexRead<int, int?>? _trackListDemuxWidth;

        /// <summary>
        /// Video height hint as indicated by the container. (Not always accurate.)
        /// </summary>
        public MpvPropertyIndexRead<int, int?> TrackListDemuxHeight => _trackListDemuxHeight ??= new MpvPropertyIndexRead<int, int?>(this, "track-list/{0}/demux-h");
        private MpvPropertyIndexRead<int, int?>? _trackListDemuxHeight;

        /// <summary>
        /// Number of audio channels as indicated by the container. (Not always accurate - in particular, the track could be decoded as a different number of channels.)
        /// </summary>
        public MpvPropertyIndexRead<int, int?> TrackListDemuxChannelCount => _trackListDemuxChannelCount ??= new MpvPropertyIndexRead<int, int?>(this, "track-list/{0}/demux-channel-count");
        private MpvPropertyIndexRead<int, int?>? _trackListDemuxChannelCount;

        /// <summary>
        /// Channel layout as indicated by the container. (Not always accurate.)
        /// </summary>
        public MpvPropertyIndexRead<int, string?> TrackListDemuxChannels => _trackListDemuxChannels ??= new MpvPropertyIndexRead<int, string?>(this, "track-list/{0}/demux-channels");
        private MpvPropertyIndexRead<int, string?>? _trackListDemuxChannels;

        /// <summary>
        /// Audio sample rate as indicated by the container. (Not always accurate.)
        /// </summary>
        public MpvPropertyIndexRead<int, int?> TrackListDemuxSampleRate => _trackListDemuxSampleRate ??= new MpvPropertyIndexRead<int, int?>(this, "track-list/{0}/demux-samplerate");
        private MpvPropertyIndexRead<int, int?>? _trackListDemuxSampleRate;

        /// <summary>
        /// Video FPS as indicated by the container. (Not always accurate.)
        /// </summary>
        public MpvPropertyIndexRead<int, float?> TrackListDemuxFps => _trackListDemuxFps ??= new MpvPropertyIndexRead<int, float?>(this, "track-list/{0}/demux-fps");
        private MpvPropertyIndexRead<int, float?>? _trackListDemuxFps;

        /// <summary>
        /// Audio average bitrate, in bits per second. (Not always accurate.)
        /// </summary>
        public MpvPropertyIndexRead<int, int?> TrackListDemuxBitrate => _trackListDemuxBitrate ??= new MpvPropertyIndexRead<int, int?>(this, "track-list/{0}/demux-bitrate");
        private MpvPropertyIndexRead<int, int?>? _trackListDemuxBitrate;

        /// <summary>
        /// Video clockwise rotation metadata, in degrees.
        /// </summary>
        public MpvPropertyIndexRead<int, int?> TrackListDemuxRotation => _trackListDemuxRotation ??= new MpvPropertyIndexRead<int, int?>(this, "track-list/{0}/demux-rotation");
        private MpvPropertyIndexRead<int, int?>? _trackListDemuxRotation;

        /// <summary>
        /// Pixel aspect ratio.
        /// </summary>
        public MpvPropertyIndexRead<int, float?> TrackListDemuxPixelAspectRatio => _trackListDemuxPixelAspectRatio ??= new MpvPropertyIndexRead<int, float?>(this, "track-list/{0}/demux-par");
        private MpvPropertyIndexRead<int, float?>? _trackListDemuxPixelAspectRatio;

        /// <summary>
        /// Per-track replaygain values. Only available for audio tracks with corresponding information stored in the source file.
        /// </summary>
        public MpvPropertyIndexRead<int, float?> TrackListReplayGainTrackPeak => _trackListReplayGainTrackPeak ??= new MpvPropertyIndexRead<int, float?>(this, "track-list/{0}/replaygain-track-peak");
        private MpvPropertyIndexRead<int, float?>? _trackListReplayGainTrackPeak;

        /// <summary>
        /// Per-track replaygain values. Only available for audio tracks with corresponding information stored in the source file.
        /// </summary>
        public MpvPropertyIndexRead<int, float?> TrackListReplayGainTrackGain => _trackListReplayGainTrackGain ??= new MpvPropertyIndexRead<int, float?>(this, "track-list/{0}/replaygain-track-gain");
        private MpvPropertyIndexRead<int, float?>? _trackListReplayGainTrackGain;

        /// <summary>
        /// Per-album replaygain values. If the file has per-track but no per-album information, the per-album values will be copied from the per-track values currently. It's possible that future mpv versions will make these properties unavailable instead in this case.
        /// </summary>
        public MpvPropertyIndexRead<int, float?> TrackListReplayGainAlbumPeak => _trackListReplayGainAlbumPeak ??= new MpvPropertyIndexRead<int, float?>(this, "track-list/{0}/replaygain-album-peak");
        private MpvPropertyIndexRead<int, float?>? _trackListReplayGainAlbumPeak;

        /// <summary>
        /// Per-album replaygain values. If the file has per-track but no per-album information, the per-album values will be copied from the per-track values currently. It's possible that future mpv versions will make these properties unavailable instead in this case.
        /// </summary>
        public MpvPropertyIndexRead<int, float?> TrackListReplayGainAlbumGain => _trackListReplayGainAlbumGain ??= new MpvPropertyIndexRead<int, float?>(this, "track-list/{0}/replaygain-album-gain");
        private MpvPropertyIndexRead<int, float?>? _trackListReplayGainAlbumGain;

        /// <summary>
        /// Returns the amount of chapters.
        /// </summary>
        public MpvPropertyRead<int?> ChapterListCount => _chapterListCount ??= new MpvPropertyRead<int?>(this, "chapter-list/count");
        private MpvPropertyRead<int?>? _chapterListCount;

        /// <summary>
        /// Chapter title as stored in the file. Not always available.
        /// </summary>
        public MpvPropertyIndexRead<int, string?> ChapterListTitle => _chapterListTitle ??= new MpvPropertyIndexRead<int, string?>(this, "chapter-list/{0}/title");
        private MpvPropertyIndexRead<int, string?>? _chapterListTitle;

        /// <summary>
        /// Chapter start time in seconds.
        /// </summary>
        public MpvPropertyIndexRead<int, double?> ChapterListTime => _chapterListTime ??= new MpvPropertyIndexRead<int, double?>(this, "chapter-list/{0}/time");
        private MpvPropertyIndexRead<int, double?>? _chapterListTime;

        /// <summary>
        /// Return whether it's generally possible to seek in the current file.
        /// </summary>
        public MpvPropertyRead<bool?> Seekable => _seekable ??= new MpvPropertyRead<bool?>(this, "seekable");
        private MpvPropertyRead<bool?>? _seekable;

        /// <summary>
        /// Return True if the current file is considered seekable, but only because the cache is active. This means small relative seeks may be fine, but larger seeks may fail anyway. Whether a seek will succeed or not is generally not known in advance.
        /// If this property returns True, seekable will also return True.
        /// </summary>
        public MpvPropertyRead<bool?> PartiallySeekable => _partiallySeekable ??= new MpvPropertyRead<bool?>(this, "partially-seekable");
        private MpvPropertyRead<bool?>? _partiallySeekable;

        /// <summary>
        /// Return whether playback is stopped or is to be stopped. (Useful in obscure situations like during on_load hook processing, when the user can stop playback, but the script has to explicitly end processing.)
        /// </summary>
        public MpvPropertyRead<bool?> PlaybackAbort => _playbackAbort ??= new MpvPropertyRead<bool?>(this, "playback-abort");
        private MpvPropertyRead<bool?>? _playbackAbort;

        /// <summary>
        /// Inserts the current OSD symbol as opaque OSD control code (cc). This makes sense only with the show-text command or options which set OSD messages. The control code is implementation specific and is useless for anything else.
        /// </summary>
        public MpvPropertyRead<string?> OsdSymCc => _osdSymCc ??= new MpvPropertyRead<string?>(this, "osd-sym-cc");
        private MpvPropertyRead<string?>? _osdSymCc;

        /// <summary>
        /// Return whether the VO is configured right now. Usually this corresponds to whether the video window is visible. If the --force-window option is used, this is usually always returns yes.
        /// </summary>
        public MpvPropertyRead<bool?> VoConfigured => _voConfigured ??= new MpvPropertyRead<bool?>(this, "vo-configured");
        private MpvPropertyRead<bool?>? _voConfigured;

        /// <summary>
        /// Contains introspection about the VO's active render passes and their execution times. Not implemented by all VOs.
        /// Fresh passes have to be uploaded, scaled, etc.
        /// </summary>
        public VideoOutputPassProperties VoPassFresh => _voPassFresh ??= new VideoOutputPassProperties(this, "vo-passes/fresh");
        private VideoOutputPassProperties? _voPassFresh;

        /// <summary>
        /// Contains introspection about the VO's active render passes and their execution times. Not implemented by all VOs.
        /// Redraw passes have to be re-painted.
        /// </summary>
        public VideoOutputPassProperties VoPassRedraw => _voPassRedraw ??= new VideoOutputPassProperties(this, "vo-passes/redraw");
        private VideoOutputPassProperties? _voPassRedraw;

        /// <summary>
        /// Bitrate values calculated on the packet level. This works by dividing the bit size of all packets between two keyframes by their presentation timestamp distance. (This uses the timestamps are stored in the file, so e.g. playback speed does not influence the returned values.) In particular, the video bitrate will update only per keyframe, and show the "past" bitrate. To make the property more UI friendly, updates to these properties are throttled in a certain way.
        /// The unit is bits per second.OSD formatting turns these values in kilobits(or megabits, if appropriate), which can be prevented by using the raw property value, e.g.with ${=video-bitrate}.
        /// </summary>
        public MpvPropertyRead<long?> VideoBitrate => _videoBitrate ??= new MpvPropertyRead<long?>(this, "video-bitrate");
        private MpvPropertyRead<long?>? _videoBitrate;

        /// <summary>
        /// Bitrate values calculated on the packet level. This works by dividing the bit size of all packets between two keyframes by their presentation timestamp distance. (This uses the timestamps are stored in the file, so e.g. playback speed does not influence the returned values.) In particular, the video bitrate will update only per keyframe, and show the "past" bitrate. To make the property more UI friendly, updates to these properties are throttled in a certain way.
        /// The unit is bits per second.OSD formatting turns these values in kilobits(or megabits, if appropriate), which can be prevented by using the raw property value, e.g.with ${=video-bitrate}.
        /// </summary>
        public MpvPropertyRead<long?> AudioBitrate => _audioBitrate ??= new MpvPropertyRead<long?>(this, "audio-bitrate");
        private MpvPropertyRead<long?>? _audioBitrate;

        /// <summary>
        /// Bitrate values calculated on the packet level. This works by dividing the bit size of all packets between two keyframes by their presentation timestamp distance. (This uses the timestamps are stored in the file, so e.g. playback speed does not influence the returned values.) In particular, the video bitrate will update only per keyframe, and show the "past" bitrate. To make the property more UI friendly, updates to these properties are throttled in a certain way.
        /// The unit is bits per second.OSD formatting turns these values in kilobits(or megabits, if appropriate), which can be prevented by using the raw property value, e.g.with ${=video-bitrate}.
        /// </summary>
        public MpvPropertyRead<long?> SubBitrate => _subBitrate ??= new MpvPropertyRead<long?>(this, "sub-bitrate");
        private MpvPropertyRead<long?>? _subBitrate;

        /// <summary>
        /// Return the list of discovered audio devices. Reflects what --audio-device=help with the command line player returns.
        /// </summary>
        public MpvPropertyRead<IList<AudioDeviceInfo>?> AudioDeviceList => _audioDeviceList ??= new MpvPropertyRead<IList<AudioDeviceInfo>?>(this, "audio-device-list");
        private MpvPropertyRead<IList<AudioDeviceInfo>?>? _audioDeviceList;

        /// <summary>
        /// Current video output driver (name as used with --vo).
        /// </summary>
        public MpvPropertyRead<string?> CurrentVideoOutput => _currentVideoOutput ??= new MpvPropertyRead<string?>(this, "current-vo");
        private MpvPropertyRead<string?>? _currentVideoOutput;

        /// <summary>
        /// Current audio output driver (name as used with --ao).
        /// </summary>
        public MpvPropertyRead<string?> CurrentAudioOutput => _currentAudioOutput ??= new MpvPropertyRead<string?>(this, "current-ao");
        private MpvPropertyRead<string?>? _currentAudioOutput;

        /// <summary>
        /// Return the working directory of the mpv process.
        /// </summary>
        public MpvPropertyRead<string?> WorkingDirectory => _workingDirectory ??= new MpvPropertyRead<string?>(this, "working-directory");
        private MpvPropertyRead<string?>? _workingDirectory;

        /// <summary>
        /// List of protocol prefixes potentially recognized by the player. They are returned without trailing :// suffix (which is still always required). In some cases, the protocol will not actually be supported (consider https if ffmpeg is not compiled with TLS support).
        /// </summary>
        public MpvPropertyRead<IList<string>?> ProtocolList => _protocolList ??= new MpvPropertyRead<IList<string>?>(this, "protocol-list");
        private MpvPropertyRead<IList<string>?>? _protocolList;

        /// <summary>
        /// List of decoders supported. This lists decoders which can be passed to --vd and --ad.
        /// </summary>
        public MpvPropertyRead<IList<DecoderInfo>?> DecoderList => _decoderList ??= new MpvPropertyRead<IList<DecoderInfo>?>(this, "decoder-list");
        private MpvPropertyRead<IList<DecoderInfo>?>? _decoderList;

        /// <summary>
        /// List of libavcodec encoders. The encoder names (driver entries) can be passed to --ovc and --oac (without the lavc: prefix required by --vd and --ad).
        /// </summary>
        public MpvPropertyRead<IList<DecoderInfo>?> EncoderList => _encoderList ??= new MpvPropertyRead<IList<DecoderInfo>?>(this, "encoder-list");
        private MpvPropertyRead<IList<DecoderInfo>?>? _encoderList;

        /// <summary>
        /// List of available libavformat demuxers' names. This can be used to check for support for a specific format or use with --demuxer-lavf-format.
        /// </summary>
        public MpvPropertyRead<IList<string>?> DemuxerLavfList => _demuxerLavfList ??= new MpvPropertyRead<IList<string>?>(this, "demuxer-lavf-list");
        private MpvPropertyRead<IList<string>?>? _demuxerLavfList;

        /// <summary>
        /// Return the mpv version/copyright string. Depending on how the binary was built, it might contain either a release version, or just a git hash.
        /// </summary>
        public MpvPropertyRead<string?> MpvVersion => _mpvVersion ??= new MpvPropertyRead<string?>(this, "mpv-version");
        private MpvPropertyRead<string?>? _mpvVersion;

        /// <summary>
        /// Return the configuration arguments which were passed to the build system (typically the way ./waf configure ... was invoked).
        /// </summary>
        public MpvPropertyRead<string?> MpvConfiguration => _mpvConfiguration ??= new MpvPropertyRead<string?>(this, "mpv-configuration");
        private MpvPropertyRead<string?>? _mpvConfiguration;

        /// <summary>
        /// Return the contents of the av_version_info() API call. This is a string which identifies the build in some way, either through a release version number, or a git hash. This applies to Libav as well (the property is still named the same.) This property is unavailable if mpv is linked against older FFmpeg and Libav versions.
        /// </summary>
        public MpvPropertyRead<string?> FFmpegVersion => _ffmpegVersion ??= new MpvPropertyRead<string?>(this, "ffmpeg-version");
        private MpvPropertyRead<string?>? _ffmpegVersion;

        /// <summary>
        /// Read-only access to value of option --<name>. Most options can be changed at runtime by writing to this property. Note that many options require reloading the file for changes to take effect. If there is an equivalent property, prefer setting the property instead.
        /// There shouldn't be any reason to access options/<name> instead of <name>, except in situations in which the properties have different behavior or conflicting semantics.
        /// </summary>
        public MpvPropertyIndexWrite<string, string?> Option => _option ??= new MpvPropertyIndexWrite<string, string?>(this, "options/{0}");
        private MpvPropertyIndexWrite<string, string?>? _option;

        /// <summary>
        /// Similar to Option, but when setting an option through this property, the option is reset to its old value once the current file has stopped playing. Trying to write an option while no file is playing (or is being loaded) results in an error.
        /// </summary>
        public MpvPropertyIndexWrite<string, string?> FileLocalOption => _fileLocalOption ??= new MpvPropertyIndexWrite<string, string?>(this, "file-local-options/{0}");
        private MpvPropertyIndexWrite<string, string?>? _fileLocalOption;

        /// <summary>
        /// Returns the name of the option.
        /// </summary>
        public MpvPropertyIndexRead<string, string?> OptionName => _optionName ??= new MpvPropertyIndexRead<string, string?>(this, "option-info/{0}/name");
        private MpvPropertyIndexRead<string, string?>? _optionName;

        /// <summary>
        /// Returns the name of the option type, like String or Integer. For many complex types, this isn't very accurate.
        /// </summary>
        public MpvPropertyIndexRead<string, string?> OptionType => _optionType ??= new MpvPropertyIndexRead<string, string?>(this, "option-info/{0}/type");
        private MpvPropertyIndexRead<string, string?>? _optionType;

        /// <summary>
        /// Return True if the option was set from the mpv command line, otherwise False. What this is set to if the option is e.g. changed at runtime is left undefined (meaning it could change in the future).
        /// </summary>
        public MpvPropertyIndexRead<string, bool?> OptionSetFromCommandLine => _optionSetFromCommandLine ??= new MpvPropertyIndexRead<string, bool?>(this, "option-info/{0}/set-from-commandline");
        private MpvPropertyIndexRead<string, bool?>? _optionSetFromCommandLine;

        /// <summary>
        /// Returns True if the option was set per-file. This is the case with automatically loaded profiles, file-dir configs, and other cases. It means the option value will be restored to the value before playback start when playback ends.
        /// </summary>
        public MpvPropertyIndexRead<string, bool?> OptionSetLocally => _optionSetLocally ??= new MpvPropertyIndexRead<string, bool?>(this, "option-info/{0}/set-locally");
        private MpvPropertyIndexRead<string, bool?>? _optionSetLocally;

        /// <summary>
        /// The default value of the option. May not always be available.
        /// </summary>
        public MpvPropertyIndexRead<string, string?> OptionDefaultValue => _optionDefaultValue ??= new MpvPropertyIndexRead<string, string?>(this, "option-info/{0}/default-value");
        private MpvPropertyIndexRead<string, string?>? _optionDefaultValue;

        /// <summary>
        /// Integer minimum value allowed for the option. Only available if the options are numeric, and the minimum/maximum has been set internally. It's also possible that only one of these is set.
        /// </summary>
        public MpvPropertyIndexRead<string, int?> OptionMin => _optionMin ??= new MpvPropertyIndexRead<string, int?>(this, "option-info/{0}/min");
        private MpvPropertyIndexRead<string, int?>? _optionMin;

        /// <summary>
        /// Integer minimum value allowed for the option. Only available if the options are numeric, and the minimum/maximum has been set internally. It's also possible that only one of these is set.
        /// </summary>
        public MpvPropertyIndexRead<string, int?> OptionMax => _optionMax ??= new MpvPropertyIndexRead<string, int?>(this, "option-info/{0}/max");
        private MpvPropertyIndexRead<string, int?>? _optionMax;

        /// <summary>
        /// If the option is a choice option, the possible choices. Choices that are integers may or may not be included (they can be implied by min and max). Note that options which behave like choice options, but are not actual choice options internally, may not have this info available.
        /// </summary>
        public MpvPropertyIndexRead<string, IList<string>?> OptionChoices => _optionChoices ??= new MpvPropertyIndexRead<string, IList<string>?>(this, "option-info/{0}/choices");
        private MpvPropertyIndexRead<string, IList<string>?>? _optionChoices;

        /// <summary>
        /// Return the list of top-level properties.
        /// </summary>
        public MpvPropertyRead<IList<string>?> PropertyList => _propertyList ??= new MpvPropertyRead<IList<string>?>(this, "property-list");
        private MpvPropertyRead<IList<string>?>? _propertyList;

        /// <summary>
        /// Return the list of profiles and their contents. This is highly implementation-specific, and may change any time. Currently, it returns an array of options for each profile. Each option has a name and a value, with the value currently always being a string. Note that the options array is not a map, as order matters and duplicate entries are possible. Recursive profiles are not expanded, and show up as special profile options.
        /// </summary>
        public MpvPropertyRead<IList<IList<KeyValuePair<string, string>>>?> ProfileList => _profileList ??= new MpvPropertyRead<IList<IList<KeyValuePair<string, string>>>?>(this, "profile-list");
        private MpvPropertyRead<IList<IList<KeyValuePair<string, string>>>?>? _profileList;

        /// <summary>
        /// Return the list of input commands. This returns an array of maps, where each map node represents a command. This map currently only has a single entry: name for the name of the command. (This property is supposed to be a replacement for --input-cmdlist. The option dumps some more information, but it's a valid feature request to extend this property if needed.)
        /// </summary>
        public MpvPropertyRead<IList<CommandInfo>?> CommandList => _commandList ??= new MpvPropertyRead<IList<CommandInfo>?>(this, "command-list");
        private MpvPropertyRead<IList<CommandInfo>?>? _commandList;

        /// <summary>
        /// Return list of current input key bindings. This returns an array of maps, where each map node represents a binding for a single key/command.
        /// </summary>
        public MpvPropertyRead<IList<CommandInfo>?> InputBindings => _inputBindings ??= new MpvPropertyRead<IList<CommandInfo>?>(this, "input-bindings");
        private MpvPropertyRead<IList<CommandInfo>?>? _inputBindings;
    }
}
