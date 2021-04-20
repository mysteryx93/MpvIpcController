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
        public MpvOptionList AudioLanguage => new(this, "alang");

        /// <summary>
        /// Specify a priority list of subtitle languages to use. Different container formats employ different language codes. DVDs use ISO 639-1 two letter language codes, Matroska uses ISO 639-2 three letter language codes while OGM uses a free-form identifier. See also --sid.
        /// </summary>
        public MpvOptionList SubLanguage => new(this, "slang");

        /// <summary>
        /// Specify a priority list of video languages to use. Different container formats employ different language codes. DVDs use ISO 639-1 two-letter language codes, Matroska, MPEG-TS and NUT use ISO 639-2 three-letter language codes, while OGM uses a free-form identifier. See also --vid.
        /// </summary>
        public MpvOptionList VideoLanguage => new(this, "vlang");

        /// <summary>
        /// Selects audio channel specified by <ID>. 'auto' selects the default, 'no' disables audio.
        /// </summary>
        public MpvOptionAutoNo<int> AudioId => new(this, "aid");

        /// <summary>
        /// Display the subtitle stream specified by <ID>. 'auto' selects the default, 'no' disables subtitles.
        /// </summary>
        public MpvOptionAutoNo<int> SubId => new(this, "sid");

        /// <summary>
        /// Select video channel specified by <ID>. 'auto' selects the default, 'no' disables video.
        /// </summary>
        public MpvOptionAutoNo<int> VideoId => new(this, "vid");

        /// <summary>
        /// (Matroska files only) Specify the edition (set of chapters) to use, where 0 is the first. If set to 'auto' (the default), mpv will choose the first edition declared as a default, or if there is no default, the first edition defined.
        /// </summary>
        public MpvOptionAuto<int> Edition => new(this, "edition");

        /// <summary>
        /// Enable the default track auto-selection (default: yes). Enabling this will make the player select streams according to --aid, --alang, and others. If it is disabled, no tracks are selected. In addition, the player will not exit if no tracks are selected, and wait instead (this wait mode is similar to pausing, but the pause option is not set).
        /// </summary>
        public MpvOption<bool> TrackAutoSelection => new(this, "track-auto-selection");

        /// <summary>
        /// When autoselecting a subtitle track, select a non-forced one even if the selected audio stream matches your preferred subtitle language (default: yes). Disable this if you'd like to only show subtitles for foreign audio or onscreen text.
        /// </summary>
        public MpvOption<bool> SubsWithMatchingAudio => new(this, "subs-with-matching-audio");

        /// <summary>
        /// Seek to given time position.
        /// The general format for times is [+|-] [[hh:] mm:]ss[.ms]. If the time is prefixed with -, the time is considered relative from the end of the file(as signaled by the demuxer/the file). A + is usually ignored(but see below).
        /// The following alternative time specifications are recognized:
        /// pp% seeks to percent position pp(0-100).
        /// #c seeks to chapter number c. (Chapters start from 1.)
        /// none resets any previously set option(useful for libmpv).
        /// </summary>
        public MpvOptionRef<string> Start => new(this, "start");

        /// <summary>
        /// Stop at given time. Use --length if the time should be relative to --start. See --start for valid option values and examples.
        /// </summary>
        public MpvOptionRef<string> End => new(this, "end");

        /// <summary>
        /// Stop after a given time relative to the start time. See --start for valid option values and examples.
        /// If both --end and --length are provided, playback will stop when it reaches either of the two endpoints.
        /// Obscurity note: this does not work correctly if --rebase-start-time=no, and the specified time is not an "absolute" time, as defined in the --start option description.
        /// </summary>
        public MpvOptionRef<string> Length => new(this, "length");

        /// <summary>
        /// Whether to move the file start time to 00:00:00 (default: yes). This is less awkward for files which start at a random timestamp, such as transport streams. On the other hand, if there are timestamp resets, the resulting behavior can be rather weird. For this reason, and in case you are actually interested in the real timestamps, this behavior can be disabled with no.
        /// </summary>
        public MpvOption<bool> RebaseStartTime => new(this, "rebase-start-time");

        /// <summary>
        /// Slow down or speed up playback by the factor given as parameter.
        /// If --audio-pitch-correction(on by default) is used, playing with a speed higher than normal automatically inserts the scaletempo2 audio filter.
        /// </summary>
        public MpvOption<float> Speed => new(this, "speed");

        /// <summary>
        /// Whether the player is in a paused state.
        /// </summary>
        public MpvOption<bool> Pause => new(this, "pause");

        /// <summary>
        /// Whether to play files in a random order.
        /// </summary>
        public MpvOption<bool> Shuffle => new(this, "shuffle");

        /// <summary>
        /// Set which file on the internal playlist to start playback with. The index is an integer, with 0 meaning the first file. The value auto means that the selection of the entry to play is left to the playback resume mechanism (default). If an entry with the given index doesn't exist, the behavior is unspecified and might change in future mpv versions. The same applies if the playlist contains further playlists (don't expect any reasonable behavior). Passing a playlist file to mpv should work with this option, though. E.g. mpv playlist.m3u --playlist-start=123 will work as expected, as long as playlist.m3u does not link to further playlists.
        /// </summary>
        public MpvOptionAuto<int> PlaylistStart => new(this, "playlist-start");

        /// <summary>
        /// Play files according to a playlist file (Supports some common formats. If no format is detected, it will be treated as list of files, separated by newline characters. Note that XML playlist formats are not supported.)
        /// You can play playlists directly and without this option, however, this option disables any security mechanisms that might be in place.You may also need this option to load plaintext files as playlist.
        /// Do NOT use Playlist with random internet sources or files you do not trust!
        /// </summary>
        public MpvOptionRef<string> Playlist => new(this, "playlist");

        /// <summary>
        /// Threshold for merging almost consecutive ordered chapter parts in milliseconds (default: 100). Some Matroska files with ordered chapters have inaccurate chapter end timestamps, causing a small gap between the end of one chapter and the start of the next one when they should match. If the end of one playback part is less than the given threshold away from the start of the next one then keep playing video normally over the chapter change instead of doing a seek.
        /// </summary>
        public MpvOption<int> ChapterMergeThreshold => new(this, "chapter-merge-threshold");

        /// <summary>
        /// Distance in seconds from the beginning of a chapter within which a backward chapter seek will go to the previous chapter (default: 5.0). Past this threshold, a backward chapter seek will go to the beginning of the current chapter instead. A negative value means always go back to the previous chapter.
        /// </summary>
        public MpvOption<double> ChapterSeekThreshold => new(this, "chapter-seek-threshold");

        /// <summary>
        /// Select when to use precise seeks that are not limited to keyframes. Such seeks require decoding video from the previous keyframe up to the target position and so can take some time depending on decoding performance. For some video formats, precise seeks are disabled. This option selects the default choice to use for seeks; it is possible to explicitly override that default in the definition of key bindings and in input commands.
        /// </summary>
        public MpvPropertyEnum<HrSeekOption> HrSeek => new(this, "hr-seek");

        /// <summary>
        /// This option exists to work around failures to do precise seeks (as in --hr-seek) caused by bugs or limitations in the demuxers for some file formats. Some demuxers fail to seek to a keyframe before the given target position, going to a later position instead. The value of this option is subtracted from the time stamp given to the demuxer. Thus, if you set this option to 1.5 and try to do a precise seek to 60 seconds, the demuxer is told to seek to time 58.5, which hopefully reduces the chance that it erroneously goes to some time later than 60 seconds. The downside of setting this option is that precise seeks become slower, as video between the earlier demuxer position and the real target may be unnecessarily decoded.
        /// </summary>
        public MpvOption<double> HrSeekDemuxerOffset => new(this, "hr-seek-demuxer-offset");

        /// <summary>
        /// Allow the video decoder to drop frames during seek, if these frames are before the seek target. If this is enabled, precise seeking can be faster, but if you're using video filters which modify timestamps or add new frames, it can lead to precise seeking skipping the target frame. This e.g. can break frame backstepping when deinterlacing is enabled.
        /// </summary>
        public MpvOption<bool> HrSeekFrameDrop => new(this, "hr-seek-framedrop");

        /// <summary>
        /// Controls how to seek in files. Note that if the index is missing from a file, it will be built on the fly by default, so you don't need to change this. But it might help with some broken files.
        /// This option only works if the underlying media supports seeking (i.e. not with stdin, pipe, etc).
        /// </summary>
        public MpvPropertyEnum<IndexMode> Index => new(this, "index");

        /// <summary>
        /// Load URLs from playlists which are considered unsafe (default: no). This includes special protocols and anything that doesn't refer to normal files. Local files and HTTP links on the other hand are always considered safe.
        /// In addition, if a playlist is loaded while this is set, the added playlist entries are not marked as originating from network or potentially unsafe location. (Instead, the behavior is as if the playlist entries were provided directly to mpv command line or loadfile command.)
        /// </summary>
        public MpvOption<bool> LoadUnsafePlaylists => new(this, "load-unsafe-playlists");

        /// <summary>
        /// Follow any references in the file being opened (default: yes). Disabling this is helpful if the file is automatically scanned (e.g. thumbnail generation). If the thumbnail scanner for example encounters a playlist file, which contains network URLs, and the scanner should not open these, enabling this option will prevent it. This option also disables ordered chapters, mov reference files, opening of archives, and a number of other features.
        /// </summary>
        public MpvOption<bool> AccessReferences => new(this, "access-references");

        /// <summary>
        /// Loops playback N times. A value of 1 plays it one time (default), 2 two times, etc. inf means forever. no is the same as 1 and disables looping. If several files are specified on command line, the entire playlist is looped. --loop-playlist is the same as --loop-playlist=inf.
        /// The force mode is like inf, but does not skip playlist entries which have been marked as failing.This means the player might waste CPU time trying to loop a file that doesn't exist. But it might be useful for playing webradios under very bad network conditions.
        /// </summary>
        public MpvOptionRef<string> LoopPlaylist => new(this, "loop-playlist");

        /// <summary>
        /// Loop a single file N times. inf means forever, no means normal playback. For compatibility, --loop-file and --loop-file=yes are also accepted, and are the same as --loop-file=inf.
        /// The difference to --loop-playlist is that this doesn't loop the playlist, just the file itself. If the playlist contains only a single file, the difference between the two option is that this option performs a seek on loop, instead of reloading the file.
        /// </summary>
        public MpvOptionRef<string> LoopFile => new(this, "loop-file");

        /// <summary>
        /// Set loop points. If playback passes the b timestamp, it will seek to the a timestamp. Seeking past the b point doesn't loop (this is intentional). If either options are set to no (or unset), looping is disabled.
        /// </summary>
        public MpvOptionRef<string> AbLoopA => new(this, "ab-loop-a");

        /// <summary>
        /// Set loop points. If playback passes the b timestamp, it will seek to the a timestamp. Seeking past the b point doesn't loop (this is intentional). If either options are set to no (or unset), looping is disabled.
        /// </summary>
        public MpvOptionRef<string> AbLoopB => new(this, "ab-loop-b");

        /// <summary>
        /// Run A-B loops only N times, then ignore the A-B loop points (default: inf). Every finished loop iteration will decrement this option by 1 (unless it is set to inf or 0). inf means that looping goes on forever. If this option is set to 0, A-B looping is ignored, and even the ab-loop command will not enable looping again (the command will show (disabled) on the OSD message if both loop points are set, but ab-loop-count is 0).
        /// </summary>
        public MpvOptionRef<string> AbLoopCount => new(this, "ab-loop-count");

        /// <summary>
        /// Enabled by default. Whether to use Matroska ordered chapters. mpv will not load or search for video segments from other files, and will also ignore any chapter order specified for the main file.
        /// </summary>
        public MpvOption<bool> OrderedChapters => new(this, "ordered-chapters");

        /// <summary>
        /// Loads the given file as playlist, and tries to use the files contained in it as reference files when opening a Matroska file that uses ordered chapters. This overrides the normal mechanism for loading referenced files by scanning the same directory the main file is located in.
        /// Useful for loading ordered chapter files that are not located on the local filesystem, or if the referenced files are in different directories.
        /// Note: a playlist can be as simple as a text file containing filenames separated by newlines.
        /// </summary>
        public MpvOptionRef<string> OrderedChaptersFiles => new(this, "ordered-chapters-files");

        /// <summary>
        /// Load chapters from this file, instead of using the chapter metadata found in the main file.
        /// This accepts a media file(like mkv) or even a pseudo-format like ffmetadata and uses its chapters to replace the current file's chapters. This doesn't work with OGM or XML chapters directly.
        /// </summary>
        public MpvOptionRef<string> ChaptersFiles => new(this, "chapters-file");

        /// <summary>
        /// Skip n seconds after every frame. Note: Without --hr-seek, skipping will snap to keyframes.
        /// </summary>
        public MpvOption<double> SkipStep => new(this, "sstep");

        /// <summary>
        /// Stop playback if either audio or video fails to initialize (default: no). With no, playback will continue in video-only or audio-only mode if one of them fails. This doesn't affect playback of audio-only or video-only files.
        /// </summary>
        public MpvOption<bool> StopPlaybackOnInitFailure => new(this, "stop-playback-on-init-failure");

        /// <summary>
        /// Control the playback direction (default: forward). Setting backward will attempt to play the file in reverse direction, with decreasing playback time. If this is set on playback starts, playback will start from the end of the file. If this is changed at during playback, a hr-seek will be issued to change the direction.
        /// + and - are aliases for forward and backward.
        /// </summary>
        public MpvOptionRef<string> PlayDir => new(this, "play-dir");

        /// <summary>
        /// For backward decoding. Backward decoding decodes forward in steps, and then reverses the decoder output. These options control the approximate maximum amount of bytes that can be buffered. The main use of this is to avoid unbounded resource usage; during normal backward playback, it's not supposed to hit the limit, and if it does, it will drop frames and complain about it.
        /// </summary>
        public MpvOption<int> VideoReversalBuffer => new(this, "video-reversal-buffer");

        /// <summary>
        /// For backward decoding. Backward decoding decodes forward in steps, and then reverses the decoder output. These options control the approximate maximum amount of bytes that can be buffered. The main use of this is to avoid unbounded resource usage; during normal backward playback, it's not supposed to hit the limit, and if it does, it will drop frames and complain about it.
        /// </summary>
        public MpvOption<int> AudioReversalBuffer => new(this, "audio-reversal-buffer");

        /// <summary>
        /// Number of overlapping keyframe ranges to use for backward decoding (default: auto) ("keyframe" to be understood as in the mpv/ffmpeg specific meaning). Backward decoding works by forward decoding in small steps. Some codecs cannot restart decoding from any packet (even if it's marked as seek point), which becomes noticeable with backward decoding (in theory this is a problem with seeking too, but --hr-seek-demuxer-offset can fix it for seeking). In particular, MDCT based audio codecs are affected.
        /// </summary>
        public MpvOptionAuto<int> VideoBackwardOverlap => new(this, "video-backward-overlap");

        /// <summary>
        /// Number of overlapping keyframe ranges to use for backward decoding (default: auto) ("keyframe" to be understood as in the mpv/ffmpeg specific meaning). Backward decoding works by forward decoding in small steps. Some codecs cannot restart decoding from any packet (even if it's marked as seek point), which becomes noticeable with backward decoding (in theory this is a problem with seeking too, but --hr-seek-demuxer-offset can fix it for seeking). In particular, MDCT based audio codecs are affected.
        /// </summary>
        public MpvOptionAuto<int> AudioBackwardOverlap => new(this, "audio-backward-overlap");

        /// <summary>
        /// Number of keyframe ranges to decode at once when backward decoding (default: 1 for video, 10 for audio). Another pointless tuning parameter nobody should use. This should affect performance only. In theory, setting a number higher than 1 for audio will reduce overhead due to less frequent backstep operations and less redundant decoding work due to fewer decoded overlap frames (see --audio-backward-overlap). On the other hand, it requires a larger reversal buffer, and could make playback less smooth due to breaking pipelining (e.g. by decoding a lot, and then doing nothing for a while).
        /// It probably never makes sense to set --video-backward-batch.But in theory, it could help with intra-only video codecs by reducing backstep operations.
        /// </summary>
        public MpvOption<int> VideoBackwardBatch => new(this, "video-backward-batch");

        /// <summary>
        /// Number of keyframe ranges to decode at once when backward decoding (default: 1 for video, 10 for audio). Another pointless tuning parameter nobody should use. This should affect performance only. In theory, setting a number higher than 1 for audio will reduce overhead due to less frequent backstep operations and less redundant decoding work due to fewer decoded overlap frames (see --audio-backward-overlap). On the other hand, it requires a larger reversal buffer, and could make playback less smooth due to breaking pipelining (e.g. by decoding a lot, and then doing nothing for a while).
        /// It probably never makes sense to set --video-backward-batch.But in theory, it could help with intra-only video codecs by reducing backstep operations.
        /// </summary>
        public MpvOption<int> AudioBackwardBatch => new(this, "audio-backward-batch");

        /// <summary>
        /// Number of seconds the demuxer should seek back to get new packets during backward playback (default: 60). This is useful for tuning backward playback, see --play-dir for details.
        /// Setting this to a very low value or 0 may make the player think seeking is broken, or may make it perform multiple seeks.
        /// Setting this to a high value may lead to quadratic runtime behavior.
        /// </summary>
        public MpvOption<double> DemuxerBackwardPlaybackStep => new(this, "demuxer-backward-playback-step");

        /// <summary>
        /// Opens the given path for writing, and print log messages to it. Existing files will be truncated. The log level is at least -v -v, but can be raised via --msg-level (the option cannot lower it below the forced minimum log level).
        /// A special case is the macOS bundle, it will create a log file at ~/Library/Logs/mpv.log by default.
        /// </summary>
        public MpvOptionRef<string> LogFile => new(this, "log-file");

        /// <summary>
        /// Force a different configuration directory. If this is set, the given directory is used to load configuration files, and all other configuration directories are ignored. This means the global mpv configuration directory as well as per-user directories are ignored, and overrides through environment variables (MPV_HOME) are also ignored.
        /// Note that the --no-config option takes precedence over this option.
        /// </summary>
        public MpvOptionRef<string> ConfigDir => new(this, "config-dir");

        /// <summary>
        /// Always save the current playback position on quit. When this file is played again later, the player will seek to the old playback position on start. This does not happen if playback of a file is stopped in any other way than quitting. For example, going to the next file in the playlist will not save the position, and start playback at beginning the next time the file is played.
        /// This behavior is disabled by default, but is always available when quitting the player with Shift+Q.
        /// </summary>
        public MpvOption<bool> SavePositionOnQuit => new(this, "save-position-on-quit");

        /// <summary>
        /// The directory in which to store the "watch later" temporary files.
        /// The default is a subdirectory named "watch_later" underneath the config directory(usually ~/.config/mpv/).
        /// </summary>
        public MpvOptionRef<string> WatchLaterDirectory => new(this, "watch-later-directory");

        /// <summary>
        /// Write certain statistics to the given file. The file is truncated on opening. The file will contain raw samples, each with a timestamp. To make this file into a readable, the script TOOLS/stats-conv.py can be used (which currently displays it as a graph).
        /// This option is useful for debugging only.
        /// </summary>
        public MpvOptionRef<string> DumpStats => new(this, "dump-stats");

        /// <summary>
        /// Yes|No|Once. Makes mpv wait idly instead of quitting when there is no file to play. Mostly useful in input mode, where mpv can be controlled through input commands. (Default: no)
        /// Once will only idle at start and let the player close once the first playlist has finished playing back.
        /// </summary>
        public MpvOptionRef<string> IdleAfterPlay => new(this, "idle");

        /// <summary>
        /// Specify configuration file to be parsed after the default ones.
        /// </summary>
        public MpvOptionRef<string> Include => new(this, "include");

        /// <summary>
        /// If set to False, don't auto-load scripts from the scripts configuration subdirectory (usually ~/.config/mpv/scripts/). (Default: True)
        /// </summary>
        public MpvOption<bool> LoadScripts => new(this, "load-scripts");

        /// <summary>
        /// Load a Lua script.
        /// </summary>
        public MpvOptionRef<string> Script => new(this, "script");

        /// <summary>
        /// Load multiple scripts by separating them with the path separator (: on Unix, ; on Windows).
        /// </summary>
        public MpvOptionList Scripts => new(this, "scripts", isPath: true);

        /// <summary>
        /// Set options for scripts. A script can query an option by key. If an option is used and what semantics the option value has depends entirely on the loaded scripts. Values not claimed by any scripts are ignored.
        /// </summary>
        public MpvOptionDictionary ScriptOptions => new(this, "script-opts");

        /// <summary>
        /// Pretend that all files passed to mpv are concatenated into a single, big file. This uses timeline/EDL support internally.
        /// </summary>
        public MpvOption<bool> MergeFiles => new(this, "merge-files");

        /// <summary>
        /// Do not restore playback position from the watch_later configuration subdirectory (usually ~/.config/mpv/watch_later/). See quit-watch-later input command.
        /// </summary>
        public MpvOption<bool> NoResumePlayback => new(this, "no-resume-playback");

        /// <summary>
        /// Only restore the playback position from the watch_later configuration subdirectory (usually ~/.config/mpv/watch_later/) if the file's modification time is the same as at the time of saving. This may prevent skipping forward in files with the same name which have different content. (Default: False)
        /// </summary>
        public MpvOption<bool> ResumePlaybackCheckMTime => new(this, "resume-playback-check-mtime");

        /// <summary>
        /// Use the given profile(s), --profile=help displays a list of the defined profiles.
        /// </summary>
        public MpvOptionList Profile => new(this, "profile");

        /// <summary>
        /// Normally, mpv will try to keep all settings when playing the next file on the playlist, even if they were changed by the user during playback. (This behavior is the opposite of MPlayer's, which tries to reset all settings when starting next file.)
        /// Default: Do not reset anything.
        /// This can be changed with this option.It accepts a list of options, and mpv will reset the value of these options on playback start to the initial value. The initial value is either the default value, or as set by the config file or command line.
        /// In some cases, this might not work as expected.For example, --volume will only be reset if it is explicitly set in the config file or the command line.
        /// The special name 'all' resets as many options as possible.
        /// </summary>
        public MpvOptionList ResetOnNextFile => new(this, "reset-on-next-file");

        /// <summary>
        /// Prepend the watch later config files with the name of the file they refer to. This is simply written as comment on the top of the file.
        /// This option may expose privacy-sensitive information and is thus disabled by default.
        /// </summary>
        public MpvOption<bool> WriteFilenameInWatchLaterConfig => new(this, "write-filename-in-watch-later-config");

        /// <summary>
        /// Ignore path (i.e. use filename only) when using watch later feature. (Default: False)
        /// </summary>
        public MpvOption<bool> IgnorePathInWatchLaterConfig => new(this, "ignore-path-in-watch-later-config");

        /// <summary>
        /// Show the description and content of a profile. Lists all profiles if no parameter is provided.
        /// </summary>
        public MpvOptionRef<string> ShowProfile => new(this, "show-profile");

        /// <summary>
        /// Look for a file-specific configuration file in the same directory as the file that is being played.
        /// Warning: May be dangerous if playing from untrusted media.
        /// </summary>
        public MpvOption<bool> UseFileDirConf => new(this, "use-filedir-conf");

        /// <summary>
        /// Enable the youtube-dl hook-script. It will look at the input URL, and will play the video located on the website. This works with many streaming sites, not just the one that the script is named after. This requires a recent version of youtube-dl to be installed on the system. (Enabled by default.)
        /// If the script can't do anything with an URL, it will do nothing.
        /// This accepts a set of options, which can be passed to it with the ScriptOpts option(using ytdl_hook- as prefix):
        /// </summary>
        public MpvOption<bool> YouTubeDl => new(this, "ytdl");

        /// <summary>
        /// yes/no. If 'yes' will try parsing the URL with youtube-dl first, instead of the default where it's only after mpv failed to open it. This mostly depends on whether most of your URLs need youtube-dl parsing.
        /// </summary>
        public MpvScriptOption YouTubeDlTryFirst => new(this, "ytdl_hook-try_ytdl_first");

        /// <summary>
        /// A |-separated list of URL patterns which mpv should not use with youtube-dl. The patterns are matched after the http(s):// part of the URL.
        /// ^ matches the beginning of the URL, $ matches its end, and you should use % before any of the characters ^$()%|,.[]*+-? to match that character.
        /// </summary>
        public MpvScriptOption YouTubeDlExclude => new(this, "ytdl_hook-exclude");

        /// <summary>
        /// yes/no. If 'yes' will attempt to add all formats found reported by youtube-dl (default: no). Each format is added as a separate track. In addition, they are delay-loaded, and actually opened only when a track is selected (this should keep load times as low as without this option).
        /// It adds average bitrate metadata, if available, which means you can use --hls-bitrate to decide which track to select. (HLS used to be the only format whose alternative quality streams were exposed in a similar way, thus the option name.)
        /// </summary>
        public MpvScriptOption YouTubeDlAllFormats => new(this, "ytdl_hook-all_formats");

        /// <summary>
        /// yes/no. If set to 'yes', and all_formats is also set to 'yes', this will try to represent all youtube-dl reported formats as tracks, even if mpv would normally use the direct URL reported by it (default: yes).
        /// It appears this normally makes a difference if youtube-dl works on a master HLS playlist.
        /// If this is set to 'no', this specific kind of stream is treated like all_formats is set to 'no', and the stream selection as done by youtube-dl (via --ytdl-format) is used.
        /// </summary>
        public MpvScriptOption YouTubeDlForceAllFormats => new(this, "ytdl_hook-force_all_formats");

        /// <summary>
        /// yes/no. Make mpv use the master manifest URL for formats like HLS and DASH, if available, allowing for video/audio selection in runtime (default: no). It's disabled ("no") by default for performance reasons.
        /// </summary>
        public MpvScriptOption YouTubeDlUseManifests => new(this, "ytdl_hook-use_manifests");

        /// <summary>
        /// Configure path to youtube-dl executable or a compatible fork's. The default "youtube-dl" looks for the executable in PATH. In a Windows environment the suffix extension ".exe" is always appended.
        /// </summary>
        public MpvScriptOption YouTubeDlPath => new(this, "ytdl_hook-ytdl_path");

        /// <summary>
        /// ytdl|best|worst|mp4|webm|... Video format/quality that is directly passed to youtube-dl. The possible values are specific to the website and the video, for a given url the available formats can be found with the command youtube-dl --list-formats URL. See youtube-dl's documentation for available aliases. (Default: bestvideo+bestaudio/best)
        /// </summary>
        public MpvOptionRef<string> YouTubeDlFormat => new(this, "ytdl-format");

        /// <summary>
        /// Pass arbitrary options to youtube-dl. Parameter and argument should be passed as a key-value pair.
        /// </summary>
        public MpvOptionDictionary YouTubeDlRawOptions => new(this, "ytdl-raw-options");

        /// <summary>
        /// Enable the builtin script that shows useful playback information on a key binding (default: True). By default, the i key is used (I to make the overlay permanent).
        /// </summary>
        public MpvOption<bool> LoadStatsOverlay => new(this, "load-stats-overlay");

        /// <summary>
        /// Enable the builtin script that shows a console on a key binding and lets you enter commands (default: True). By default, the ´ key is used to show the console, and ESC to hide it again. (This is based on a user script called repl.lua.)
        /// </summary>
        public MpvOption<bool> LoadOsdConsole => new(this, "load-osd-console");

        /// <summary>
        /// Enable the builtin script that does auto profiles (default: auto). See Conditional auto profiles for details. auto will load the script, but immediately unload it if there are no conditional profiles.
        /// </summary>
        public MpvOptionAuto<bool> LoadAutoProfiles => new(this, "load-auto-profiles");

        /// <summary>
        /// cplayer|pseudo-gui. For enabling "pseudo GUI mode", which means that the defaults for some options are changed. This option should not normally be used directly, but only by mpv internally, or mpv-provided scripts, config files, or .desktop files. See PSEUDO GUI MODE for details.
        /// </summary>
        public MpvOptionRef<string> PlayerOperationMode => new(this, "player-operation-mode");

        public MpvOption<double> Volume => new(this, "volume");
    }
}
