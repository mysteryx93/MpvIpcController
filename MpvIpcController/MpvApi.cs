using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    // MPV JSON IPC protocol documentation
    // https://mpv.io/manual/stable/#json-ipc

    /// <summary>
    /// Exposes MPV's API in a strongly-typed way.
    /// </summary>
    public class MpvApi : IDisposable
    {
        private readonly IMpvController _mpv;

        public MpvApi(IMpvController controller)
        {
            _mpv = controller;
        }

        /// <summary>
        /// Gets the controller handling API requests.
        /// </summary>
        public IMpvController Controller => _mpv;

        private static T ConvertValue<T>(object? value)
            where T : struct
        {
            return value == null ? default : (T)value;
        }

        /// <summary>
        /// Returns the name of the client as string. This is the string ipc-N with N being an integer number.
        /// </summary>
        public async Task<string> GetClientNameAsync()
        {
            var response = await _mpv.SendMessageAsync(null, "client_name").ConfigureAwait(false);
            return response?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Returns the current mpv internal time in microseconds as a number. This is basically the system time, with an arbitrary offset.
        /// </summary>
        public async Task<int> GetClientTimeAsync()
        {
            var response = await _mpv.SendMessageAsync(null, "get_time_us").ConfigureAwait(false);
            return ConvertValue<int>(response);
        }

        /// <summary>
        /// Returns the value of the given property. The value will be sent in the data field of the replay message.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        public async Task<T> GetPropertyAsync<T>(string propertyName, MpvCommandOptions? options = null)
            where T : struct
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            var response = await _mpv.SendMessageAsync(options, "get_property", propertyName).ConfigureAwait(false);
            return ConvertValue<T>(response);
        }

        /// <summary>
        /// Returns the value of the given property. The resulting data will always be a string.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        public async Task<string> GetPropertyStringAsync(string propertyName, MpvCommandOptions? options = null)
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            var response = await _mpv.SendMessageAsync(options, "get_property_string", propertyName).ConfigureAwait(false);
            return response?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Sets the given property to the given value.
        /// </summary>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public async Task SetPropertyAsync(string propertyName, object value, MpvCommandOptions? options = null)
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            await _mpv.SendMessageAsync(options, "set_property", propertyName, value).ConfigureAwait(false);
        }

        /// <summary>
        /// Watches a property for changes. If the given property is changed, then an event 'property-change' will be generated.
        /// </summary>
        /// <param name="observeId">An ID that will be passed to the generated events as parameter 'id'.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        public async Task ObservePropertyAsync(int observeId, string propertyName, MpvCommandOptions? options = null)
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            await _mpv.SendMessageAsync(options, "observe_property", observeId, propertyName).ConfigureAwait(false);
        }

        /// <summary>
        /// Watches a property for changes. If the given property is changed, then an event 'property-change' will be generated. The resulting data will always be a string.
        /// </summary>
        /// <param name="observeId">An ID for the observer.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        public async Task ObservePropertyStringAsync(int observeId, string propertyName, MpvCommandOptions? options = null)
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            await _mpv.SendMessageAsync(options, "observe_property_string", observeId, propertyName).ConfigureAwait(false);
        }

        /// <summary>
        /// Undo ObserveProperty or ObservePropertyString. This requires the numeric id passed to the observed command as argument.
        /// </summary>
        /// <param name="observeId">The ID of the observer.</param>
        public async Task UnobservePropertyAsync(int observeId, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "observe_property", observeId).ConfigureAwait(false);
        }

        /// <summary>
        /// Enable output of mpv log messages. They will be received as events.
        /// Log message output is meant for humans only (mostly for debugging). Attempting to retrieve information by parsing these messages will just lead to breakages with future mpv releases.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Reviewed: API is expecting lowercase log-level.")]
        public async Task RequestLogMessagesAsync(LogLevel logLevel, MpvCommandOptions? options = null)
        {
            logLevel.CheckEnumValid(nameof(logLevel));

            await _mpv.SendMessageAsync(options, "request_log_messages", logLevel.FormatMpvFlag()).ConfigureAwait(false);
        }

        /// <summary>
        /// Enables the named event. If the string 'all' is used instead of an event name, all events are enabled.
        /// By default, most events are enabled, and there is not much use for this command.
        /// </summary>
        /// <param name="eventName">The name of the event to enable.</param>
        public async Task EnableEventAsync(string eventName, MpvCommandOptions? options = null)
        {
            eventName.CheckNotNullOrEmpty(nameof(eventName));

            await _mpv.SendMessageAsync(options, "enable_event", eventName).ConfigureAwait(false);
        }

        /// <summary>
        /// Disables the named event. If the string 'all' is used instead of an event name, all events are disabled.
        /// </summary>
        /// <param name="eventName">The name of the event to enable.</param>
        public async Task DisableEventAsync(string eventName, MpvCommandOptions? options = null)
        {
            eventName.CheckNotNullOrEmpty(nameof(eventName));

            await _mpv.SendMessageAsync(options, "disable_event", eventName).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the client API version the C API of the remote mpv instance provides.
        /// </summary>
        public async Task<int> GetVersionAsync(MpvCommandOptions? options = null)
        {
            var response = await _mpv.SendMessageAsync(options, "get_version").ConfigureAwait(false);
            return ConvertValue<int>(response);
        }

        /// <summary>
        /// Use this to "block" keys that should be unbound, and do nothing. Useful for disabling default bindings, without disabling all bindings with --no-input-default-bindings.
        /// </summary>
        public async Task IgnoreKeysAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "ignore").ConfigureAwait(false);
        }

        /// <summary>
        /// Changes the playback position. By default, seeks by a relative amount of seconds.
        /// </summary>
        /// <param name="units">The amount of units to seek. Seconds by default.</param>
        /// <param name="flags">Flags controlling the seek mode. Flags can be combined.</param>
        public async Task SeekAsync(double units, SeekOptions flags, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "seek", units, flags.GetFlags().FormatMpvFlag()).ConfigureAwait(false);
        }

        /// <summary>
        /// Undoes the seek command, and some other commands that seek (but not necessarily all of them). Calling this command once will jump to the playback position before the seek. Calling it a second time undoes the revert-seek command itself. This only works within a single file.
        /// </summary>
        /// <param name="mark">If true, marks the current time position. The next normal revert-seek command will seek back to this point, no matter how many seeks happened since last time.</param>
        public async Task RevertSeekAsync(bool mark, MpvCommandOptions? options = null)
        {
            var strFlags = mark ? "mark" : null;
            await _mpv.SendMessageAsync(options, "revert-seek", strFlags).ConfigureAwait(false);
        }

        /// <summary>
        /// Plays one frame, then pause. Does nothing with audio-only playback.
        /// </summary>
        public async Task FrameStepAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "frame-step").ConfigureAwait(false);
        }

        /// <summary>
        /// Go back by one frame, then pause. Note that this can be very slow (it tries to be precise, not fast), and sometimes fails to behave as expected. How well this works depends on whether precise seeking works correctly (e.g. see the --hr-seek-demuxer-offset option). Video filters or other video post-processing that modifies timing of frames (e.g. deinterlacing) should usually work, but might make backstepping silently behave incorrectly in corner cases. Using --hr-seek-framedrop=no should help, although it might make precise seeking slower.
        /// This does not work with audio-only playback.
        /// </summary>
        public async Task FrameBackStepAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "frame-back-step").ConfigureAwait(false);
        }

        /// <summary>
        /// Set the given property or option to the given value.
        /// </summary>
        /// <param name="name">The name of a property or option.</param>
        /// <param name="value">The value to set.</param>
        public async Task SetAsync(string name, object value, MpvCommandOptions? options = null)
        {
            name.CheckNotNullOrEmpty(nameof(name));

            await _mpv.SendMessageAsync(options, "set", name, value).ConfigureAwait(false);
        }

        /// <summary>
        /// Add the given value to the property or option. On overflow or underflow, clamp the property to the maximum. If <value> is omitted, assume 1.
        /// </summary>
        /// <param name="name">The name of a property or option.</param>
        /// <param name="value">The value to add.</param>
        public async Task AddAsync(string name, object value, MpvCommandOptions? options = null)
        {
            name.CheckNotNullOrEmpty(nameof(name));
            value.CheckNotNull(nameof(value));

            await _mpv.SendMessageAsync(options, "add", name, value).ConfigureAwait(false);
        }

        /// <summary>
        /// Cycles the given property or option. The second argument can be up or down to set the cycle direction. On overflow, set the property back to the minimum, on underflow set it to the maximum.
        /// </summary>
        /// <param name="name">The name of a property or option.</param>
        /// <param name="direction">The cycling direction. By default, Up.</param>
        /// <returns></returns>
        public async Task CycleAsync(string name, CycleDirection direction = CycleDirection.Up, MpvCommandOptions? options = null)
        {
            name.CheckNotNullOrEmpty(nameof(name));
            direction.CheckEnumValid(nameof(direction));

            await _mpv.SendMessageAsync(options, "cycle", name, direction).ConfigureAwait(false);
        }

        /// <summary>
        /// Similar to add, but multiplies the property or option with the numeric value.
        /// </summary>
        /// <param name="name">The name of a property or option.</param>
        /// <param name="value">The multiplication factor.</param>
        public async Task MultiplyAsync(string name, object value, MpvCommandOptions? options = null)
        {
            name.CheckNotNullOrEmpty(nameof(name));

            await _mpv.SendMessageAsync(options, "multiply", name, value).ConfigureAwait(false);
        }

        /// <summary>
        /// Takes a screenshot.
        /// </summary>
        /// <param name="flags">Screenshot options. Multiple flags can be combined.</param>
        public async Task ScreenshotAsync(ScreenshotOptions flags, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "screenshot", flags.GetFlags().FormatMpvFlag()).ConfigureAwait(false);
        }

        /// <summary>
        /// Takes a screenshot and save it to a given file. If the file already exists, it's overwritten. 
        /// The format of the file will be guessed by the extension (and --screenshot-format is ignored - the behavior when the extension is missing or unknown is arbitrary).
        /// </summary>
        /// <param name="fileName">The file name where to save the screenshot.</param>
        /// <param name="flags">Screenshot options. Multiple flags can be combined.</param>
        public async Task ScreenshotToFileAsync(string fileName, ScreenshotOptions flags, MpvCommandOptions? options = null)
        {
            fileName.CheckNotNullOrEmpty(nameof(fileName));

            await _mpv.SendMessageAsync(options, "screenshot-to-file", fileName, flags.GetFlags().FormatMpvFlag()).ConfigureAwait(false);
        }

        /// <summary>
        /// Goes to the next entry on the playlist.
        /// </summary>
        /// <param name="terminateOnEnd">When the last file is being played, if true, ends playback, otherwise does nothing.</param>
        public async Task PlaylistNextAsync(bool terminateOnEnd = false, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "playlist-next", terminateOnEnd ? "force" : null).ConfigureAwait(false);
        }

        /// <summary>
        /// Goes to the previous entry on the playlist.
        /// </summary>
        /// <param name="terminateOnEnd">When the first file is being played, if true, ends playback, otherwise does nothing.</param>
        public async Task PlaylistPrevAsync(bool terminateOnEnd = false, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "playlist-prev", terminateOnEnd ? "force" : null).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the given file or URL and plays it.
        /// </summary>
        /// <param name="path">The path of the file or URL to play.</param>
        /// <param name="append">Append the file to the playlist.</param>
        /// <param name="appendPlay">Append the file, and if nothing is currently playing, start playback. (Always starts with the added file, even if the playlist was not empty before running this command.)</param>
        /// <param name="extraArgs">A list of options and values which should be set while the file is playing. Not all options can be changed this way. Some options require a restart of the player.</param>
        public async Task LoadFileAsync(string path, bool append = false, bool appendPlay = false, IDictionary<string, object>? extraArgs = null, MpvCommandOptions? options = null)
        {
            path.CheckNotNullOrEmpty(nameof(path));

            var flag = append ? (appendPlay ? "append-play" : "append") : "replace";
            var args = new object[3 + (extraArgs?.Count ?? 0)];
            args[0] = "loadfile";
            args[1] = path;
            args[2] = flag;
            if (extraArgs != null)
            {
                var index = 2;
                foreach (var item in extraArgs)
                {
                    // Note: options and values are not validated and will be passed as-is.
                    args[index++] = item.Key + "=" + item.Value.ToStringInvariant();
                }
            }
            await _mpv.SendMessageAsync(options, args).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the given playlist file or URL.
        /// </summary>
        /// <param name="path">The path of the playlist file or URL to play.</param>
        /// <param name="append">Append the new playlist at the end of the current internal playlist.</param>
        public async Task LoadPlaylistAsync(string path, bool append = false, MpvCommandOptions? options = null)
        {
            path.CheckNotNullOrEmpty(nameof(path));

            var flag = append ? "append" : "replace";
            await _mpv.SendMessageAsync(options, "loadlist", path, flag).ConfigureAwait(false);
        }

        /// <summary>
        /// Clears the playlist, except the currently played file.
        /// </summary>
        public async Task PlaylistClearAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "playlist-clear").ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the playlist entry at the given index. Index values start counting with 0.
        /// </summary>
        /// <param name="index">The index of the playlist entry to remove.</param>
        public async Task PlaylistRemoveAsync(int index, MpvCommandOptions? options = null)
        {
            index.CheckRange(nameof(index), min: 0);

            await _mpv.SendMessageAsync(options, "playlist-remove", index).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the current playlist entry, stops playback and starts playing the next entry.
        /// </summary>
        /// <param name="index">The index of the playlist entry to remove.</param>
        public async Task PlaylistRemoveCurrentAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "playlist-remove", "current").ConfigureAwait(false);
        }

        /// <summary>
        /// Move the playlist entry at index1, so that it takes the place of the entry index2. (Paradoxically, the moved playlist entry will not have the index value index2 after moving if index1 was lower than index2, because index2 refers to the target entry, not the index the entry will have after moving.)
        /// </summary>
        /// <param name="index">The index of the playlist entry to move.</param>
        /// <param name="destIndex">The index to move the destination to.</param>
        public async Task PlaylistMoveAsync(int index, int destIndex, MpvCommandOptions? options = null)
        {
            index.CheckRange(nameof(index), min: 0);
            destIndex.CheckRange(nameof(destIndex), min: 0);

            await _mpv.SendMessageAsync(options, "playlist-move", index, destIndex).ConfigureAwait(false);
        }

        /// <summary>
        /// Shuffle the playlist.
        /// </summary>
        public async Task PlaylistShuffleAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "playlist-shuffle").ConfigureAwait(false);
        }

        /// <summary>
        /// Attempt to revert the previous PlaylistShuffle command. This works only once (multiple successive PlaylistUnshuffle commands do nothing). May not work correctly if new recursive playlists have been opened since a PlaylistShuffle command.
        /// </summary>
        public async Task PlaylistUnshuffleAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "playlist-unshuffle").ConfigureAwait(false);
        }

        /// <summary>
        /// Runs the given shell command.
        /// </summary>
        /// <param name="command">The command to run.</param>
        public async Task RunShellAsync(string command, MpvCommandOptions? options = null)
        {
            command.CheckNotNullOrEmpty(nameof(command));

            await _mpv.SendMessageAsync(options, "run", "/bin/sh", "-c", command).ConfigureAwait(false);
        }

        /// <summary>
        /// Exits the player. If an argument is given, it's used as process exit code.
        /// </summary>
        /// <param name="exitCode">The process exit code.</param>
        public async Task QuitAsync(int? exitCode = null, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "quit", exitCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Exits the player, and store current playback position. Playing that file later will seek to the previous position on start.
        /// </summary>
        /// <param name="exitCode">The process exit code.</param>
        public async Task QuitWatchLaterAsync(int? exitCode = null, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "quit-watch-later", exitCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the given subtitle file or stream. By default, it is selected as current subtitle after loading.
        /// </summary>
        /// <param name="path">The file path or URL of the subtitle to load.</param>
        /// <param name="option">Subtitle loading options.</param>
        /// <param name="title">The track title in the UI.</param>
        /// <param name="lang">The track language. It can influence stream selection when using LoadOption.Auto.</param>
        public async Task SubAddAsync(string path, LoadOption option = LoadOption.Select, string? title = null, string? lang = null, MpvCommandOptions? options = null)
        {
            path.CheckNotNullOrEmpty(nameof(path));
            option.CheckEnumValid(nameof(option));

            await _mpv.SendMessageAsync(options, "sub-add", path, option.FormatMpvFlag(), title, lang).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the given subtitle track. If the id argument is missing, remove the current track. (Works on external subtitle files only.)
        /// </summary>
        /// <param name="id">The ID of the subtitle track to remove.</param>
        public async Task SubRemoveAsync(int? id = null, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "sub-remove", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Reloads the given subtitle tracks. If the id argument is missing, reload the current track. (Works on external subtitle files only.)
        /// This works by unloading and re-adding the subtitle track.
        /// </summary>
        /// <param name="id">The ID of the subtitle track to reload.</param>
        public async Task SubReloadAsync(int? id = null, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "sub-reload", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Changes subtitle timing such, that the subtitle event after the next <skip> subtitle events is displayed. <skip> can be negative to step backwards.
        /// </summary>
        /// <param name="skip">The amount of subtitle events to move to, forwards or backwards.</param>
        public async Task SubStepAsync(int skip, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "sub-step", skip).ConfigureAwait(false);
        }

        /// <summary>
        /// Seeks to the next (skip set to 1) or the previous (skip set to -1) subtitle. This is similar to sub-step, except that it seeks video and audio instead of adjusting the subtitle delay.
        /// For embedded subtitles(like with Matroska), this works only with subtitle events that have already been displayed, or are within a short prefetch range.
        /// </summary>
        /// <param name="skip">The amount of subtitle events to move to, forwards or backwards.</param>
        public async Task SubSeekAsync(int skip, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "sub-seek", skip).ConfigureAwait(false);
        }

        /// <summary>
        /// Prints text to stdout. The string can contain properties (see Property Expansion).
        /// </summary>
        /// <param name="text">The text to print to stdout.</param>
        public async Task PrintTextAsync(string text, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "print-text", text).ConfigureAwait(false);
        }

        /// <summary>
        /// Shows text on the OSD. The string can contain properties, which are expanded as described in Property Expansion. This can be used to show playback time, filename, and so on.
        /// </summary>
        /// <param name="text">The OSD text to show.</param>
        /// <param name="duration">The time in ms to show the message for. By default, it uses the same value as --osd-duration.</param>
        /// <param name="minOsdLevel">The minimum OSD level to show the text at (see --osd-level).</param>
        /// <returns></returns>
        public async Task ShowTextAsync(string text, int duration, int minOsdLevel, MpvCommandOptions? options = null)
        {
            minOsdLevel.CheckRange(nameof(minOsdLevel), min: 1, max: 3);

            await _mpv.SendMessageAsync(options, "show-text", text, duration, minOsdLevel).ConfigureAwait(false);
        }

        /// <summary>
        /// Shows the progress bar, the elapsed time and the total duration of the file on the OSD.
        /// </summary>
        public async Task ShowProgressAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "show-progress").ConfigureAwait(false);
        }

        /// <summary>
        /// Writes the resume config file that the QuitWatchLater command writes, but continue playback normally.
        /// </summary>
        public async Task WriteWatchLaterConfigAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "write-watch-later-config").ConfigureAwait(false);
        }

        /// <summary>
        /// Stops playback and clear playlist. With default settings, this is essentially like quit. Useful for the client API: playback can be stopped without terminating the player.
        /// </summary>
        public async Task StopAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "stop").ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a mouse event with given coordinate (x, y).
        /// </summary>
        /// <param name="x">The horizontal mouse coordinate to send.</param>
        /// <param name="y">The vertical mouse coordinate to send.</param>
        /// <param name="button">The button number of clicked mouse button. This should be one of 0-19. If omitted, only the position will be updated.</param>
        /// <param name="doubleClick">If true, the mouse event represents double-click, otherwise regular single-click.</param>
        /// <returns></returns>
        public async Task MouseAsync(int x, int y, int? button = null, bool doubleClick = false, MpvCommandOptions? options = null)
        {
            x.CheckRange(nameof(x), min: 0);
            y.CheckRange(nameof(y), min: 0);
            if (button.HasValue)
            {
                button.Value.CheckRange(nameof(button), min: 0, max: 19);
            }

            await _mpv.SendMessageAsync(options, "mouse", x, y, button, doubleClick ? "double" : null).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a key event through mpv's input handler, triggering whatever behavior is configured to that key. name uses the input.conf naming scheme for keys and modifiers. Useful for the client API: key events can be sent to libmpv to handle internally.
        /// </summary>
        /// <param name="key">The key to send.</param>
        public async Task KeyPressAsync(string key, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "keypress", key).ConfigureAwait(false);
        }

        /// <summary>
        /// Similar to KeyPress, but sets the KEYDOWN flag so that if the key is bound to a repeatable command, it will be run repeatedly with mpv's key repeat timing until the keyup command is called.
        /// </summary>
        /// <param name="key">The key to send.</param>
        public async Task KeyDownAsync(string key, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "keydown", key).ConfigureAwait(false);
        }

        /// <summary>
        /// Set the KEYUP flag, stopping any repeated behavior that had been triggered. If key is null, KEYUP will be set on all keys. Otherwise, KEYUP will only be set on the key specified by name.
        /// </summary>
        /// <param name="key">The key to send.</param>
        public async Task KeyUpAsync(string? key = null, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "keyup", key).ConfigureAwait(false);
        }

        /// <summary>
        /// Binds a key to an input command. command must be a complete command containing all the desired arguments and flags. Both name and command use the input.conf naming scheme. This is primarily useful for the client API.
        /// </summary>
        /// <param name="key">The key to bind.</param>
        /// <param name="command">The full command to trigger containing all the desired arguments and flags.</param>
        public async Task KeyBindAsync(string key, string command, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "keybind", key, command).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the given audio file or stream. By default, it is selected as current subtitle after loading.
        /// </summary>
        /// <param name="path">The file path or URL of the audio to load.</param>
        /// <param name="option">Audio loading options.</param>
        /// <param name="title">The track title in the UI.</param>
        /// <param name="lang">The track language. It can influence stream selection when using LoadOption.Auto.</param>
        public async Task AudioAddAsync(string path, LoadOption option = LoadOption.Select, string? title = null, string? lang = null, MpvCommandOptions? options = null)
        {
            path.CheckNotNullOrEmpty(nameof(path));
            option.CheckEnumValid(nameof(option));

            await _mpv.SendMessageAsync(options, "audio-add", path, option.FormatMpvFlag(), title, lang).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the given audio track. If the id argument is missing, remove the current track. (Works on external audio files only.)
        /// </summary>
        /// <param name="id">The ID of the audio track to remove.</param>
        public async Task AudioRemoveAsync(int? id = null, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "audio-remove", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Reloads the given audio tracks. If the id argument is missing, reload the current track. (Works on external audio files only.)
        /// This works by unloading and re-adding the audio track.
        /// </summary>
        /// <param name="id">The ID of the audio track to reload.</param>
        public async Task AudioReloadAsync(int? id = null, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "audio-reload", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the given video file or stream. By default, it is selected as current video after loading.
        /// </summary>
        /// <param name="path">The file path or URL of the video to load.</param>
        /// <param name="option">Video loading options.</param>
        /// <param name="title">The track title in the UI.</param>
        /// <param name="lang">The track language. It can influence stream selection when using LoadOption.Auto.</param>
        public async Task VideoAddAsync(string path, LoadOption option = LoadOption.Select, string? title = null, string? lang = null, MpvCommandOptions? options = null)
        {
            path.CheckNotNullOrEmpty(nameof(path));
            option.CheckEnumValid(nameof(option));

            await _mpv.SendMessageAsync(options, "video-add", path, option.FormatMpvFlag(), title, lang).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the given video track. If the id argument is missing, remove the current track. (Works on external video files only.)
        /// </summary>
        /// <param name="id">The ID of the video track to remove.</param>
        public async Task VideoRemoveAsync(int? id = null, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "video-remove", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Reloads the given video tracks. If the id argument is missing, reload the current track. (Works on external video files only.)
        /// This works by unloading and re-adding the video track.
        /// </summary>
        /// <param name="id">The ID of the video track to reload.</param>
        public async Task VideoReloadAsync(int? id = null, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "video-reload", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Rescans external files according to the current --sub-auto and --audio-file-auto settings. This can be used to auto-load external files after the file was loaded.
        /// </summary>
        /// <param name="reselect">If true, select the default audio and subtitle streams, which typically selects external files with the highest preference. If false, do not change current track selections.</param>
        public async Task RescanExternalFilesAsync(bool reselect = true, MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "rescan-external-files", reselect ? "reselect" : "keep-selection").ConfigureAwait(false);
        }


        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _mpv.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
