using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvApi : MpvController, IMpvApi
    {
        /// <summary>
        /// Initializes a new instance of the MpvController class to handle communication over specified stream.
        /// </summary>
        /// <param name="connection">A stream supporting both reading and writing.</param>
        public MpvApi(NamedPipeClientStream connection) : base(connection)
        {
        }

        private static T ConvertValue<T>(object? value)
            where T : struct
        {
            return value == null ? default : (T)value;
        }

        /// <summary>
        /// Returns the name of the client as string. This is the string ipc-N with N being an integer number.
        /// </summary>
        public async Task<string> GetClientName()
        {
            var response = await SendMessageAsync("client_name").ConfigureAwait(false);
            return response?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Returns the current mpv internal time in microseconds as a number. This is basically the system time, with an arbitrary offset.
        /// </summary>
        public async Task<int> GetClientTime()
        {
            var response = await SendMessageAsync("get_time_us").ConfigureAwait(false);
            return ConvertValue<int>(response);
        }

        /// <summary>
        /// Returns the value of the given property. The value will be sent in the data field of the replay message.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        public async Task<T> GetProperty<T>(string propertyName)
            where T : struct
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            var response = await SendMessageAsync("get_property", propertyName).ConfigureAwait(false);
            return ConvertValue<T>(response);
        }

        /// <summary>
        /// Returns the value of the given property. The resulting data will always be a string.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        public async Task<string> GetPropertyString(string propertyName)
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            var response = await SendMessageAsync("get_property_string", propertyName).ConfigureAwait(false);
            return response?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Sets the given property to the given value.
        /// </summary>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public async Task SetProperty(string propertyName, object value)
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            await SendMessageAsync("set_property", propertyName, value).ConfigureAwait(false);
        }

        /// <summary>
        /// Watches a property for changes. If the given property is changed, then an event 'property-change' will be generated.
        /// </summary>
        /// <param name="observeId">An ID that will be passed to the generated events as parameter 'id'.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        public async Task ObserveProperty(int observeId, string propertyName)
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            await SendMessageAsync("observe_property", observeId, propertyName).ConfigureAwait(false);
        }

        /// <summary>
        /// Watches a property for changes. If the given property is changed, then an event 'property-change' will be generated. The resulting data will always be a string.
        /// </summary>
        /// <param name="observeId">An ID for the observer.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        public async Task ObservePropertyString(int observeId, string propertyName)
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            await SendMessageAsync("observe_property_string", observeId, propertyName).ConfigureAwait(false);
        }

        /// <summary>
        /// Undo ObserveProperty or ObservePropertyString. This requires the numeric id passed to the observed command as argument.
        /// </summary>
        /// <param name="observeId">The ID of the observer.</param>
        public async Task UnobserveProperty(int observeId)
        {
            await SendMessageAsync("observe_property", observeId).ConfigureAwait(false);
        }

        /// <summary>
        /// Enable output of mpv log messages. They will be received as events.
        /// Log message output is meant for humans only (mostly for debugging). Attempting to retrieve information by parsing these messages will just lead to breakages with future mpv releases.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Reviewed: API is expecting lowercase log-level.")]
        public async Task RequestLogMessages(LogLevel logLevel)
        {
            logLevel.CheckEnumValid(nameof(logLevel));

            await SendMessageAsync("request_log_messages", logLevel.ToString().ToLowerInvariant()).ConfigureAwait(false);
        }

        /// <summary>
        /// Enables the named event. If the string 'all' is used instead of an event name, all events are enabled.
        /// By default, most events are enabled, and there is not much use for this command.
        /// </summary>
        /// <param name="eventName">The name of the event to enable.</param>
        public async Task EnableEvent(string eventName)
        {
            eventName.CheckNotNullOrEmpty(nameof(eventName));

            await SendMessageAsync("enable_event", eventName).ConfigureAwait(false);
        }

        /// <summary>
        /// Disables the named event. If the string 'all' is used instead of an event name, all events are disabled.
        /// </summary>
        /// <param name="eventName">The name of the event to enable.</param>
        public async Task DisableEvent(string eventName)
        {
            eventName.CheckNotNullOrEmpty(nameof(eventName));

            await SendMessageAsync("disable_event", eventName).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the client API version the C API of the remote mpv instance provides.
        /// </summary>
        public async Task<int> GetVersion()
        {
            var response = await SendMessageAsync("get_version").ConfigureAwait(false);
            return ConvertValue<int>(response);
        }

        /// <summary>
        /// Use this to "block" keys that should be unbound, and do nothing. Useful for disabling default bindings, without disabling all bindings with --no-input-default-bindings.
        /// </summary>
        public async Task IgnoreKeys()
        {
            await SendMessageAsync("ignore").ConfigureAwait(false);
        }

        /// <summary>
        /// Changes the playback position. By default, seeks by a relative amount of seconds.
        /// </summary>
        /// <param name="units">The amount of units to seek. Seconds by default.</param>
        /// <param name="flags">Flags controlling the seek mode. Flags can be combined.</param>
        public async Task Seek(double units, params SeekOption[] flags)
        {
            await SendMessageAsync("seek", units, flags.NormalizeFlags()).ConfigureAwait(false);
        }

        /// <summary>
        /// Undoes the seek command, and some other commands that seek (but not necessarily all of them). Calling this command once will jump to the playback position before the seek. Calling it a second time undoes the revert-seek command itself. This only works within a single file.
        /// </summary>
        /// <param name="mark">If true, marks the current time position. The next normal revert-seek command will seek back to this point, no matter how many seeks happened since last time.</param>
        public async Task RevertSeek(bool mark)
        {
            var strFlags = mark ? "mark" : null;
            await SendMessageAsync("revert-seek", strFlags).ConfigureAwait(false);
        }

        /// <summary>
        /// Plays one frame, then pause. Does nothing with audio-only playback.
        /// </summary>
        public async Task FrameStep()
        {
            await SendMessageAsync("frame-step").ConfigureAwait(false);
        }

        /// <summary>
        /// Go back by one frame, then pause. Note that this can be very slow (it tries to be precise, not fast), and sometimes fails to behave as expected. How well this works depends on whether precise seeking works correctly (e.g. see the --hr-seek-demuxer-offset option). Video filters or other video post-processing that modifies timing of frames (e.g. deinterlacing) should usually work, but might make backstepping silently behave incorrectly in corner cases. Using --hr-seek-framedrop=no should help, although it might make precise seeking slower.
        /// This does not work with audio-only playback.
        /// </summary>
        public async Task FrameBackStep()
        {
            await SendMessageAsync("frame-back-step").ConfigureAwait(false);
        }

        /// <summary>
        /// Set the given property or option to the given value.
        /// </summary>
        /// <param name="name">The name of a property or option.</param>
        /// <param name="value">The value to set.</param>
        public async Task Set(string name, object value)
        {
            name.CheckNotNullOrEmpty(nameof(name));

            await SendMessageAsync("set", name, value).ConfigureAwait(false);
        }

        /// <summary>
        /// Add the given value to the property or option. On overflow or underflow, clamp the property to the maximum. If <value> is omitted, assume 1.
        /// </summary>
        /// <param name="name">The name of a property or option.</param>
        /// <param name="value">The value to add.</param>
        public async Task Add(string name, object value)
        {
            name.CheckNotNullOrEmpty(nameof(name));
            value.CheckNotNull(nameof(value));

            await SendMessageAsync("add", name, value).ConfigureAwait(false);
        }

        /// <summary>
        /// Cycles the given property or option. The second argument can be up or down to set the cycle direction. On overflow, set the property back to the minimum, on underflow set it to the maximum.
        /// </summary>
        /// <param name="name">The name of a property or option.</param>
        /// <param name="direction">The cycling direction. By default, Up.</param>
        /// <returns></returns>
        public async Task Cycle(string name, CycleDirection direction = CycleDirection.Up)
        {
            name.CheckNotNullOrEmpty(nameof(name));
            direction.CheckEnumValid(nameof(direction));

            await SendMessageAsync("cycle", name, direction).ConfigureAwait(false);
        }

        /// <summary>
        /// Similar to add, but multiplies the property or option with the numeric value.
        /// </summary>
        /// <param name="name">The name of a property or option.</param>
        /// <param name="value">The multiplication factor.</param>
        public async Task Multiply(string name, object value)
        {
            name.CheckNotNullOrEmpty(nameof(name));

            await SendMessageAsync("multiply", name, value).ConfigureAwait(false);
        }

        /// <summary>
        /// Takes a screenshot.
        /// </summary>
        /// <param name="flags">Screenshot options. Multiple flags can be combined.</param>
        public async Task Screenshot(params ScreenshotOption[] flags)
        {
            await SendMessageAsync("screenshot", flags.NormalizeFlags()).ConfigureAwait(false);
        }

        /// <summary>
        /// Takes a screenshot and save it to a given file. If the file already exists, it's overwritten. 
        /// The format of the file will be guessed by the extension (and --screenshot-format is ignored - the behavior when the extension is missing or unknown is arbitrary).
        /// </summary>
        /// <param name="fileName">The file name where to save the screenshot.</param>
        /// <param name="flags">Screenshot options. Multiple flags can be combined.</param>
        public async Task ScreenshotToFile(string fileName, params ScreenshotOption[] flags)
        {
            fileName.CheckNotNullOrEmpty(nameof(fileName));

            await SendMessageAsync("screenshot-to-file", fileName, flags.NormalizeFlags()).ConfigureAwait(false);
        }

        /// <summary>
        /// Goes to the next entry on the playlist.
        /// </summary>
        /// <param name="terminateOnEnd">When the last file is being played, if true, ends playback, otherwise does nothing.</param>
        public async Task PlaylistNext(bool terminateOnEnd = false)
        {
            await SendMessageAsync("playlist-next", terminateOnEnd ? "force" : null).ConfigureAwait(false);
        }

        /// <summary>
        /// Goes to the previous entry on the playlist.
        /// </summary>
        /// <param name="terminateOnEnd">When the first file is being played, if true, ends playback, otherwise does nothing.</param>
        public async Task PlaylistPrev(bool terminateOnEnd = false)
        {
            await SendMessageAsync("playlist-prev", terminateOnEnd ? "force" : null).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the given file or URL and plays it.
        /// </summary>
        /// <param name="path">The path of the file or URL to play.</param>
        /// <param name="append">Append the file to the playlist.</param>
        /// <param name="appendPlay">Append the file, and if nothing is currently playing, start playback. (Always starts with the added file, even if the playlist was not empty before running this command.)</param>
        /// <param name="options">A list of options and values which should be set while the file is playing. Not all options can be changed this way. Some options require a restart of the player.</param>
        public async Task LoadFile(string path, bool append = false, bool appendPlay = false, IDictionary<string, object>? options = null)
        {
            path.CheckNotNullOrEmpty(nameof(path));

            var flag = append ? (appendPlay ? "append-play" : "append") : "replace";
            var args = new object[2 + options?.Count ?? 0];
            args[0] = path;
            args[1] = flag;
            if (options != null)
            {
                var index = 2;
                foreach (var item in options)
                {
                    // Note: options and values are not validated and will be passed as-is.
                    args[index++] = item.Key + "=" + item.Value.ToStringInvariant();
                }
            }
            await SendMessageAsync("loadfile", args).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the given playlist file or URL.
        /// </summary>
        /// <param name="path">The path of the playlist file or URL to play.</param>
        /// <param name="append">Append the new playlist at the end of the current internal playlist.</param>
        public async Task LoadPlaylist(string path, bool append = false)
        {
            path.CheckNotNullOrEmpty(nameof(path));

            var flag = append ? "append" : "replace";
            await SendMessageAsync("loadlist", path, flag).ConfigureAwait(false);
        }

        /// <summary>
        /// Clears the playlist, except the currently played file.
        /// </summary>
        public async Task PlaylistClear()
        {
            await SendMessageAsync("playlist-clear").ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the playlist entry at the given index. Index values start counting with 0.
        /// </summary>
        /// <param name="index">The index of the playlist entry to remove.</param>
        public async Task PlaylistRemove(int index)
        {
            index.CheckRange(nameof(index), min: 0);

            await SendMessageAsync("playlist-remove", index).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the current playlist entry, stops playback and starts playing the next entry.
        /// </summary>
        /// <param name="index">The index of the playlist entry to remove.</param>
        public async Task PlaylistRemoveCurrent()
        {
            await SendMessageAsync("playlist-remove", "current").ConfigureAwait(false);
        }

        /// <summary>
        /// Move the playlist entry at index1, so that it takes the place of the entry index2. (Paradoxically, the moved playlist entry will not have the index value index2 after moving if index1 was lower than index2, because index2 refers to the target entry, not the index the entry will have after moving.)
        /// </summary>
        /// <param name="index">The index of the playlist entry to move.</param>
        /// <param name="destIndex">The index to move the destination to.</param>
        public async Task PlaylistMove(int index, int destIndex)
        {
            index.CheckRange(nameof(index), min: 0);
            destIndex.CheckRange(nameof(destIndex), min: 0);

            await SendMessageAsync("playlist-move", index, destIndex).ConfigureAwait(false);
        }

        /// <summary>
        /// Shuffle the playlist.
        /// </summary>
        public async Task PlaylistShuffle()
        {
            await SendMessageAsync("playlist-shuffle").ConfigureAwait(false);
        }

        /// <summary>
        /// Attempt to revert the previous PlaylistShuffle command. This works only once (multiple successive PlaylistUnshuffle commands do nothing). May not work correctly if new recursive playlists have been opened since a PlaylistShuffle command.
        /// </summary>
        public async Task PlaylistUnshuffle()
        {
            await SendMessageAsync("playlist-unshuffle").ConfigureAwait(false);
        }

        /// <summary>
        /// Runs the given shell command.
        /// </summary>
        /// <param name="command">The command to run.</param>
        public async Task RunShell(string command)
        {
            command.CheckNotNullOrEmpty(nameof(command));

            await SendMessageAsync("run", "/bin/sh", "-c", command).ConfigureAwait(false);
        }

        /// <summary>
        /// Exits the player. If an argument is given, it's used as process exit code.
        /// </summary>
        /// <param name="exitCode">The process exit code.</param>
        public async Task Quit(int? exitCode = null)
        {
            await SendMessageAsync("quit", exitCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Exits the player, and store current playback position. Playing that file later will seek to the previous position on start.
        /// </summary>
        /// <param name="exitCode">The process exit code.</param>
        public async Task QuitWatchLater(int? exitCode = null)
        {
            await SendMessageAsync("quit-watch-later", exitCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the given subtitle file or stream. By default, it is selected as current subtitle after loading.
        /// </summary>
        /// <param name="path">The file path or URL of the subtitle to load.</param>
        /// <param name="option">Subtitle loading options.</param>
        /// <param name="title">The track title in the UI.</param>
        /// <param name="lang">The track language. It can influence stream selection when using LoadOption.Auto.</param>
        public async Task SubAdd(string path, LoadOption option = LoadOption.Select, string? title = null, string? lang = null)
        {
            path.CheckNotNullOrEmpty(nameof(path));
            option.CheckEnumValid(nameof(option));

            await SendMessageAsync("sub-add", path, option.GetFlagName(), title, lang).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the given subtitle track. If the id argument is missing, remove the current track. (Works on external subtitle files only.)
        /// </summary>
        /// <param name="id">The ID of the subtitle track to remove.</param>
        public async Task SubRemove(int? id = null)
        {
            await SendMessageAsync("sub-remove", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Reloads the given subtitle tracks. If the id argument is missing, reload the current track. (Works on external subtitle files only.)
        /// This works by unloading and re-adding the subtitle track.
        /// </summary>
        /// <param name="id">The ID of the subtitle track to reload.</param>
        public async Task SubReload(int? id = null)
        {
            await SendMessageAsync("sub-reload", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Changes subtitle timing such, that the subtitle event after the next <skip> subtitle events is displayed. <skip> can be negative to step backwards.
        /// </summary>
        /// <param name="skip">The amount of subtitle events to move to, forwards or backwards.</param>
        public async Task SubStep(int skip)
        {
            await SendMessageAsync("sub-step", skip).ConfigureAwait(false);
        }

        /// <summary>
        /// Seeks to the next (skip set to 1) or the previous (skip set to -1) subtitle. This is similar to sub-step, except that it seeks video and audio instead of adjusting the subtitle delay.
        /// For embedded subtitles(like with Matroska), this works only with subtitle events that have already been displayed, or are within a short prefetch range.
        /// </summary>
        /// <param name="skip">The amount of subtitle events to move to, forwards or backwards.</param>
        public async Task SubSeek(int skip)
        {
            await SendMessageAsync("sub-seek", skip).ConfigureAwait(false);
        }

        /// <summary>
        /// Prints text to stdout. The string can contain properties (see Property Expansion).
        /// </summary>
        /// <param name="text">The text to print to stdout.</param>
        public async Task PrintText(string text)
        {
            await SendMessageAsync("print-text", text).ConfigureAwait(false);
        }

        /// <summary>
        /// Shows text on the OSD. The string can contain properties, which are expanded as described in Property Expansion. This can be used to show playback time, filename, and so on.
        /// </summary>
        /// <param name="text">The OSD text to show.</param>
        /// <param name="duration">The time in ms to show the message for. By default, it uses the same value as --osd-duration.</param>
        /// <param name="minOsdLevel">The minimum OSD level to show the text at (see --osd-level).</param>
        /// <returns></returns>
        public async Task ShowText(string text, int duration, int minOsdLevel)
        {
            minOsdLevel.CheckRange(nameof(minOsdLevel), min: 1, max: 3);

            await SendMessageAsync("show-text", text, duration, minOsdLevel).ConfigureAwait(false);
        }

        /// <summary>
        /// Shows the progress bar, the elapsed time and the total duration of the file on the OSD.
        /// </summary>
        public async Task ShowProgress()
        {
            await SendMessageAsync("show-progress").ConfigureAwait(false);
        }

        /// <summary>
        /// Writes the resume config file that the QuitWatchLater command writes, but continue playback normally.
        /// </summary>
        public async Task WriteWatchLaterConfig()
        {
            await SendMessageAsync("write-watch-later-config").ConfigureAwait(false);
        }

        /// <summary>
        /// Stops playback and clear playlist. With default settings, this is essentially like quit. Useful for the client API: playback can be stopped without terminating the player.
        /// </summary>
        public async Task Stop()
        {
            await SendMessageAsync("stop").ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a mouse event with given coordinate (x, y).
        /// </summary>
        /// <param name="x">The horizontal mouse coordinate to send.</param>
        /// <param name="y">The vertical mouse coordinate to send.</param>
        /// <param name="button">The button number of clicked mouse button. This should be one of 0-19. If omitted, only the position will be updated.</param>
        /// <param name="doubleClick">If true, the mouse event represents double-click, otherwise regular single-click.</param>
        /// <returns></returns>
        public async Task Mouse(int x, int y, int? button = null, bool doubleClick = false)
        {
            x.CheckRange(nameof(x), min: 0);
            y.CheckRange(nameof(y), min: 0);
            if (button.HasValue)
            {
                button.Value.CheckRange(nameof(button), min: 0, max: 19);
            }

            await SendMessageAsync("mouse", x, y, button, doubleClick ? "double" : null).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a key event through mpv's input handler, triggering whatever behavior is configured to that key. name uses the input.conf naming scheme for keys and modifiers. Useful for the client API: key events can be sent to libmpv to handle internally.
        /// </summary>
        /// <param name="key">The key to send.</param>
        public async Task KeyPress(string key)
        {
            await SendMessageAsync("keypress", key).ConfigureAwait(false);
        }

        /// <summary>
        /// Similar to KeyPress, but sets the KEYDOWN flag so that if the key is bound to a repeatable command, it will be run repeatedly with mpv's key repeat timing until the keyup command is called.
        /// </summary>
        /// <param name="key">The key to send.</param>
        public async Task KeyDown(string key)
        {
            await SendMessageAsync("keydown", key).ConfigureAwait(false);
        }

        /// <summary>
        /// Set the KEYUP flag, stopping any repeated behavior that had been triggered. If key is null, KEYUP will be set on all keys. Otherwise, KEYUP will only be set on the key specified by name.
        /// </summary>
        /// <param name="key">The key to send.</param>
        public async Task KeyUp(string? key = null)
        {
            await SendMessageAsync("keyup", key).ConfigureAwait(false);
        }

        /// <summary>
        /// Binds a key to an input command. command must be a complete command containing all the desired arguments and flags. Both name and command use the input.conf naming scheme. This is primarily useful for the client API.
        /// </summary>
        /// <param name="key">The key to bind.</param>
        /// <param name="command">The full command to trigger containing all the desired arguments and flags.</param>
        public async Task KeyBind(string key, string command)
        {
            await SendMessageAsync("keybind", key, command).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the given audio file or stream. By default, it is selected as current subtitle after loading.
        /// </summary>
        /// <param name="path">The file path or URL of the audio to load.</param>
        /// <param name="option">Audio loading options.</param>
        /// <param name="title">The track title in the UI.</param>
        /// <param name="lang">The track language. It can influence stream selection when using LoadOption.Auto.</param>
        public async Task AudioAdd(string path, LoadOption option = LoadOption.Select, string? title = null, string? lang = null)
        {
            path.CheckNotNullOrEmpty(nameof(path));
            option.CheckEnumValid(nameof(option));

            await SendMessageAsync("audio-add", path, option.GetFlagName(), title, lang).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the given audio track. If the id argument is missing, remove the current track. (Works on external audio files only.)
        /// </summary>
        /// <param name="id">The ID of the audio track to remove.</param>
        public async Task AudioRemove(int? id = null)
        {
            await SendMessageAsync("audio-remove", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Reloads the given audio tracks. If the id argument is missing, reload the current track. (Works on external audio files only.)
        /// This works by unloading and re-adding the audio track.
        /// </summary>
        /// <param name="id">The ID of the audio track to reload.</param>
        public async Task AudioReload(int? id = null)
        {
            await SendMessageAsync("audio-reload", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the given video file or stream. By default, it is selected as current video after loading.
        /// </summary>
        /// <param name="path">The file path or URL of the video to load.</param>
        /// <param name="option">Video loading options.</param>
        /// <param name="title">The track title in the UI.</param>
        /// <param name="lang">The track language. It can influence stream selection when using LoadOption.Auto.</param>
        public async Task VideoAdd(string path, LoadOption option = LoadOption.Select, string? title = null, string? lang = null)
        {
            path.CheckNotNullOrEmpty(nameof(path));
            option.CheckEnumValid(nameof(option));

            await SendMessageAsync("video-add", path, option.GetFlagName(), title, lang).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the given video track. If the id argument is missing, remove the current track. (Works on external video files only.)
        /// </summary>
        /// <param name="id">The ID of the video track to remove.</param>
        public async Task VideoRemove(int? id = null)
        {
            await SendMessageAsync("video-remove", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Reloads the given video tracks. If the id argument is missing, reload the current track. (Works on external video files only.)
        /// This works by unloading and re-adding the video track.
        /// </summary>
        /// <param name="id">The ID of the video track to reload.</param>
        public async Task VideoReload(int? id = null)
        {
            await SendMessageAsync("video-reload", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Rescans external files according to the current --sub-auto and --audio-file-auto settings. This can be used to auto-load external files after the file was loaded.
        /// </summary>
        /// <param name="reselect">If true, select the default audio and subtitle streams, which typically selects external files with the highest preference. If false, do not change current track selections.</param>
        public async Task RescanExternalFiles(bool reselect = true)
        {
            await SendMessageAsync("rescan-external-files", reselect ? "reselect" : "keep-selection").ConfigureAwait(false);
        }
    }
}
