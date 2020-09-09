using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

        //private static T? ConvertValue<T>(object? value, T? defaultValue)
        //    where T : struct
        //{
        //    return value == null ? defaultValue : (T)value;
        //}

        //private static T? ConvertValue<T>(object? value, T? defaultValue)
        //    where T : class
        //{
        //    return value == null ? defaultValue : (T)value;
        //}

        /// <summary>
        /// Returns the name of the client as string. This is the string ipc-N with N being an integer number.
        /// </summary>
        public async Task<string> GetClientNameAsync()
        {
            return await _mpv.SendMessageAsync(null, "client_name").ConfigureAwait(false) ?? string.Empty;
        }

        /// <summary>
        /// Returns the current mpv internal time in microseconds as a number. This is basically the system time, with an arbitrary offset.
        /// </summary>
        public async Task<int> GetClientTimeAsync()
        {
            return await _mpv.SendMessageAsync<int>(null, "get_time_us").ConfigureAwait(false) ?? 0;
        }

        /// <summary>
        /// Returns the value of the given property as a nullable value type.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        public async Task<T?> GetPropertyAsync<T>(string propertyName)
            where T : struct
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            return await _mpv.SendMessageAsync<T>(null, "get_property", propertyName).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the value of the given property as a class type.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        public async Task<T?> GetPropertyClassAsync<T>(string propertyName)
            where T : class
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            return await _mpv.SendMessageClassAsync<T>(null, "get_property", propertyName).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the value of the given property. The resulting data will always be a string.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        public async Task<string> GetPropertyStringAsync(string propertyName)
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));

            return await _mpv.SendMessageAsync(null, "get_property_string", propertyName).ConfigureAwait(false) ?? string.Empty;
        }

        /// <summary>
        /// Sets the given property to the given value.
        /// </summary>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public async Task SetPropertyAsync(string propertyName, object? value, MpvCommandOptions? options = null)
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
        public async Task<int?> GetVersionAsync(MpvCommandOptions? options = null)
        {
            return await _mpv.SendMessageAsync<int>(options, "get_version").ConfigureAwait(false);
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
        public async Task SetAsync(string name, object? value, MpvCommandOptions? options = null)
        {
            name.CheckNotNullOrEmpty(nameof(name));

            await _mpv.SendMessageAsync(options, "set", name, value).ConfigureAwait(false);
        }

        /// <summary>
        /// Add the given value to the property or option. On overflow or underflow, clamp the property to the maximum. If <value> is omitted, assume 1.
        /// </summary>
        /// <param name="name">The name of a property or option.</param>
        /// <param name="value">The value to add.</param>
        public async Task AddAsync(string name, object? value, MpvCommandOptions? options = null)
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
        public async Task MultiplyAsync(string name, object? value, MpvCommandOptions? options = null)
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

        /// <summary>
        /// Changes the audio filter chain.
        /// </summary>
        /// <param name="operation">Decides what happens to the filter.</param>
        /// <param name="value">The value to apply with the operation.</param>
        public async Task AudioFilterAsync(FilterOperation operation, string value = "", MpvCommandOptions? options = null)
        {
            operation.CheckEnumValid(nameof(operation));

            await _mpv.SendMessageAsync(options, "af", operation.FormatMpvFlag(), value).ConfigureAwait(false);
        }

        /// <summary>
        /// Changes the video filter chain.
        /// </summary>
        /// <param name="operation">Decides what happens to the filter.</param>
        /// <param name="value">The value to apply with the operation.</param>
        public async Task VideoFilterAsync(FilterOperation operation, string value = "", MpvCommandOptions? options = null)
        {
            operation.CheckEnumValid(nameof(operation));

            await _mpv.SendMessageAsync(options, "vf", operation.FormatMpvFlag(), value).ConfigureAwait(false);
        }

        /// <summary>
        /// Cycles through a list of values. Each invocation of the command will set the given property to the next value in the list. The command will use the current value of the property/option, and use it to determine the current position in the list of values. Once it has found it, it will set the next value in the list (wrapping around to the first item if needed).
        /// </summary>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="values">The list of values to set.</param>
        /// <param name="reverse">Can be used to cycle the value list in reverse. The only advantage is that you don't need to reverse the value list yourself when adding a second key binding for cycling backwards.</param>
        public async Task CycleValues(string propertyName, IEnumerable<object> values, bool reverse = false, MpvCommandOptions? options = null)
        {
            propertyName.CheckNotNullOrEmpty(nameof(propertyName));
            values.CheckNotNullOrEmpty(nameof(values));

            var args = new object?[values.Count() + (reverse ? 3 : 2)];
            args[0] = "cycle-values";
            var i = 1;
            if (reverse)
            {
                args[i++] = "!reverse";
            }
            args[i++] = propertyName;
            foreach (var item in values)
            {
                args[i++] = item;
            }

            await _mpv.SendMessageAsync(options, args).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds an OSD overlay sourced from raw data. This might be useful for scripts and applications controlling mpv, and which want to display things on top of the video window.
        /// </summary>
        /// <param name="id">Identify the overlay element between 0 and 63. The ID can be used to add multiple overlay parts, update a part by using this command with an already existing ID, or to remove a part with overlay-remove. Using a previously unused ID will add a new overlay, while reusing an ID will update it.</param>
        /// <param name="x">Specify the horizontal position where the OSD should be displayed.</param>
        /// <param name="y">Specify the vertical position where the OSD should be displayed.</param>
        /// <param name="w">The visible width of the overlay.</param>
        /// <param name="h">The visible height of the overlay.</param>
        /// <param name="file">Specify the file the raw image data is read from. It can be either a numeric UNIX file descriptor prefixed with @ (e.g. @4), or a filename. The file will be mapped into memory with mmap(), copied, and unmapped before the command returns (changed in mpv 0.18.1). 
        /// It is also possible to pass a raw memory address for use as bitmap memory by passing a memory address as integer prefixed with an & character. Passing the wrong thing here will crash the player. This mode might be useful for use with libmpv. The offset parameter is simply added to the memory address (since mpv 0.8.0, ignored before).</param>
        /// <param name="stride">Gives the width in bytes in memory./param>
        /// <param name="offset">The byte offset of the first pixel in the source file. (The current implementation always mmap's the whole file from position 0 to the end of the image, so large offsets should be avoided. Before mpv 0.8.0, the offset was actually passed directly to mmap, but it was changed to make using it easier.)</param>
        /// <param name="fmt">String identifying the image format. Currently, only bgra is defined. This format has 4 bytes per pixels, with 8 bits per component. The least significant 8 bits are blue, and the most significant 8 bits are alpha (in little endian, the components are B-G-R-A, with B as first byte). This uses premultiplied alpha: every color component is already multiplied with the alpha component. This means the numeric value of each component is equal to or smaller than the alpha component. (Violating this rule will lead to different results with different VOs: numeric overflows resulting from blending broken alpha values is considered something that shouldn't happen, and consequently implementations don't ensure that you get predictable behavior in this case.)</param>
        public async Task ImageOverlayAdd(int id, int x, int y, int w, int h, string file, int stride, int offset = 0, string fmt = "bgra", MpvCommandOptions? options = null)
        {
            id.CheckRange(nameof(id), min: 0, max: 63);
            w.CheckRange(nameof(w), min: 0);
            h.CheckRange(nameof(h), min: 0);
            stride.CheckRange(nameof(stride), min: 0);
            file.CheckNotNullOrEmpty(nameof(file));

            await _mpv.SendMessageAsync(options, "overlay-add", id, x, y, w, h, file, stride, offset, fmt).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes an overlay added with overlay-add and the same ID. Does nothing if no overlay with this ID exists.
        /// </summary>
        /// <param name="id">The id of the overlay to remove.</param>
        public async Task ImageOverlayRemove(int id, MpvCommandOptions? options = null)
        {
            id.CheckRange(nameof(id), min: 0, max: 63);

            await _mpv.SendMessageAsync(options, "overlay-remove", id).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds/updates/removes an OSD overlay.
        /// (Although this sounds similar to overlay-add, osd-overlay is for text overlays, while overlay-add is for bitmaps.Maybe overlay-add will be merged into osd-overlay to remove this oddity.)
        /// If the libmpv client is destroyed, all overlays associated with it are also deleted. In particular, connecting via --input-ipc-server, adding an overlay, and disconnecting will remove the overlay immediately again.
        /// </summary>
        /// <param name="id">Arbitrary integer that identifies the overlay. Multiple overlays can be added by calling this command with different id parameters. Calling this command with the same id replaces the previously set overlay.
        /// There is a separate namespace for each libmpv client (i.e. IPC connection, script), so IDs can be made up and assigned by the API user without conflicting with other API users.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        //public async Task TextOverlayAdd(int id = 0, string format = "", MpvCommandOptions? options = null)
        //{
        //    await _mpv.SendMessageAsync(options, "osd-overlay", id).ConfigureAwait(false);
        //}

        //public async Task TextOverlayRemove(int id = 0, MpvCommandOptions? options = null)
        //{
        //    await _mpv.SendMessageAsync(options, "osd-overlay", id).ConfigureAwait(false);
        //}


        /// <summary>
        /// Sends a message to all clients, and pass it the following list of arguments. What this message means, how many arguments it takes, and what the arguments mean is fully up to the receiver and the sender. Every client receives the message, so be careful about name clashes (or use script-message-to).
        /// This command has a variable number of arguments, and cannot be used with named arguments.
        /// </summary>
        /// <param name="args">The values to send to other applications.</param>
        public async Task ScriptMessageAsync(IEnumerable<object?> args, MpvCommandOptions? options = null)
        {
            args.CheckNotNullOrEmpty(nameof(args));

            var values = new object[args.Count() + 1];
            values[0] = "script-message";
            var i = 1;
            foreach (var item in args)
            {
                values[i++] = args;
            }
            await _mpv.SendMessageAsync(options, values).ConfigureAwait(false);
        }

        /// <summary>
        /// Same as script-message, but sends it only to the client named <target>. Each client (scripts etc.) has a unique name. For example, Lua scripts can get their name via mp.get_script_name().
        /// This command has a variable number of arguments, and cannot be used with named arguments.
        /// </summary>
        /// <param name="target">The name of the api client to send the message to, which can be obtained with get_script_name.</param>
        /// <param name="args">The values to send to the other application.</param>
        public async Task ScriptMessageToAsync(string target, IEnumerable<object?> args, MpvCommandOptions? options = null)
        {
            args.CheckNotNullOrEmpty(nameof(args));

            var values = new object[args.Count() + 2];
            values[0] = "script-message-to";
            values[1] = target;
            var i = 2;
            foreach (var item in args)
            {
                values[i++] = args;
            }
            await _mpv.SendMessageAsync(options, values).ConfigureAwait(false);
        }

        /// <summary>
        /// Invokes a script-provided key binding. This can be used to remap key bindings provided by external Lua scripts.
        /// </summary>
        /// <param name="name">The name of the binding.</param>
        public async Task ScriptBindingAsync(string name, MpvCommandOptions? options = null)
        {
            name.CheckNotNullOrEmpty(nameof(name));

            await _mpv.SendMessageAsync(options, "script-binding", name).ConfigureAwait(false);
        }

        /// <summary>
        /// Cycles through A-B loop states. The first command will set the A point (the ab-loop-a property); the second the B point, and the third will clear both points.
        /// </summary>
        public async Task AbLoopAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "ab-loop").ConfigureAwait(false);
        }

        /// <summary>
        /// Drops audio/video/demuxer buffers, and restart from fresh. Might help with unseekable streams that are going out of sync. This command might be changed or removed in the future.
        /// </summary>
        public async Task DropBuffersAsync(MpvCommandOptions? options = null)
        {
            await _mpv.SendMessageAsync(options, "drop-buffers").ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a command to the video filter with the given label. The command and argument string is filter specific. Currently, this only works with the lavfi filter - see the libavfilter documentation for which commands a filter supports.
        /// Note that the label is a mpv filter label, not a libavfilter filter name.
        /// </summary>
        /// <param name="label">The label of the filter, or 'all' to send to all filters at once.</param>
        /// <param name="command">The command to send, which is filter-specific.</param>
        /// <param name="argument">The argument to send with the command.</param>
        public async Task VideoFilterCommandAsync(string label, string command, string argument, MpvCommandOptions? options = null)
        {
            label.CheckNotNullOrEmpty(nameof(label));
            command.CheckNotNullOrEmpty(nameof(command));
            argument.CheckNotNullOrEmpty(nameof(argument));

            await _mpv.SendMessageAsync(options, "vf-command", label, command, argument).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a command to the audio filter with the given label. The command and argument string is filter specific. Currently, this only works with the lavfi filter - see the libavfilter documentation for which commands a filter supports.
        /// Note that the label is a mpv filter label, not a libavfilter filter name.
        /// </summary>
        /// <param name="label">The label of the filter, or 'all' to send to all filters at once.</param>
        /// <param name="command">The command to send, which is filter-specific.</param>
        /// <param name="argument">The argument to send with the command.</param>
        public async Task AudioFilterCommandAsync(string label, string command, string argument, MpvCommandOptions? options = null)
        {
            label.CheckNotNullOrEmpty(nameof(label));
            command.CheckNotNullOrEmpty(nameof(command));
            argument.CheckNotNullOrEmpty(nameof(argument));

            await _mpv.SendMessageAsync(options, "af-command", label, command, argument).ConfigureAwait(false);
        }

        /// <summary>
        /// Applies the contents of a named profile. This is like using profile=name in a config file, except you can map it to a key binding to change it at runtime.
        /// There is no such thing as "unapplying" a profile - applying a profile merely sets all option values listed within the profile.
        /// </summary>
        /// <param name="name">The name of the profile to set.</param>
        public async Task ApplyProfileAsync(string name, MpvCommandOptions? options = null)
        {
            name.CheckNotNullOrEmpty(nameof(name));

            await _mpv.SendMessageAsync(options, "apply-profile", name).ConfigureAwait(false);
        }

        /// <summary>
        /// Load a script, similar to the --script option. Whether this waits for the script to finish initialization or not changed multiple times, and the future behavior is left undefined.
        /// </summary>
        /// <param name="fileName">The file path of the scrit to load.</param>
        public async Task LoadScriptAsync(string fileName, MpvCommandOptions? options = null)
        {
            fileName.CheckNotNullOrEmpty(nameof(fileName));

            await _mpv.SendMessageAsync(options, "load-script", fileName).ConfigureAwait(false);
        }

        /// <summary>
        /// Changes list options as described in List Options.
        /// Some operations take no value, but the command still requires the value parameter. In these cases, the value must be an empty string.
        /// </summary>
        /// <param name="name">The normal option name.</param>
        /// <param name="operation">The suffix or action used on the option.</param>
        /// <param name="value">The value to apply the operation with.</param>
        public async Task ChangeListAsync(string name, ListOptionOperation operation, string value = "", MpvCommandOptions? options = null)
        {
            name.CheckNotNullOrEmpty(nameof(name));
            operation.CheckEnumValid(nameof(operation));

            await _mpv.SendMessageAsync(options, "change-list", name, operation.FormatMpvFlag(), value).ConfigureAwait(false);
        }

        /// <summary>
        /// Factor multiplied with speed at which the player attempts to play the file. Usually it's exactly 1. (Display sync mode will make this useful.)
        /// </summary>
        public MpvPropertyRead<float> AudioSpeedCorrection => _audioSpeedCorrection ??= new MpvPropertyRead<float>(this, "audio-speed-correction", 1);
        private MpvPropertyRead<float>? _audioSpeedCorrection;

        /// <summary>
        /// Factor multiplied with speed at which the player attempts to play the file. Usually it's exactly 1. (Display sync mode will make this useful.)
        /// </summary>
        public MpvPropertyRead<float> VideoSpeedCorrection => _videoSpeedCorrection ??= new MpvPropertyRead<float>(this, "video-speed-correction", 1);
        private MpvPropertyRead<float>? _videoSpeedCorrection;

        /// <summary>
        /// Return whether --video-sync=display is actually active.
        /// </summary>
        public MpvPropertyRead<bool> DisplaySyncActive => _displaySyncActive ??= new MpvPropertyRead<bool>(this, "display-sync-active", false);
        private MpvPropertyRead<bool>? _displaySyncActive;

        /// <summary>
        /// Currently played file, with path stripped. If this is an URL, try to undo percent encoding as well. (The result is not necessarily correct, but looks better for display purposes. Use the path property to get an unmodified filename.)
        /// </summary>
        public MpvPropertyReadClass<string> FileName => _fileName ??= new MpvPropertyReadClass<string>(this, "filename", string.Empty);
        private MpvPropertyReadClass<string>? _fileName;

        /// <summary>
        /// Like the filename property, but if the text contains a ., strip all text after the last .. Usually this removes the file extension.
        /// </summary>
        public MpvPropertyReadClass<string> FileNameNoExt => _fileNameNoExt ??= new MpvPropertyReadClass<string>(this, "filename/no-ext", string.Empty);
        private MpvPropertyReadClass<string>? _fileNameNoExt;

        /// <summary>
        /// Length in bytes of the source file/stream. (This is the same as ${stream-end}. For segmented/multi-part files, this will return the size of the main or manifest file, whatever it is.)
        /// </summary>
        public MpvPropertyRead<long> FileSize => _fileSize ??= new MpvPropertyRead<long>(this, "file-size", 0);
        private MpvPropertyRead<long>? _fileSize;

        /// <summary>
        /// Total number of frames in current file. This is only an estimate. (It's computed from two unreliable quantities: fps and stream length.)
        /// </summary>
        public MpvPropertyRead<long> EstimatedFrameCount => _estimatedFrameCount ??= new MpvPropertyRead<long>(this, "estimated-frame-count", 0);
        private MpvPropertyRead<long>? _estimatedFrameCount;

        /// <summary>
        /// Number of current frame in current stream.This is only an estimate. (It's computed from two unreliable quantities: fps and possibly rounded timestamps.)
        /// </summary>
        public MpvPropertyRead<long> EstimatedFrameNumber => _estimatedFrameNumber ??= new MpvPropertyRead<long>(this, "estimated-frame-number", 0);
        private MpvPropertyRead<long>? _estimatedFrameNumber;

        /// <summary>
        /// Full path of the currently played file. Usually this is exactly the same string you pass on the mpv command line or the loadfile command, even if it's a relative path. If you expect an absolute path, you will have to determine it yourself, for example by using the working-directory property.
        /// </summary>
        public MpvPropertyReadClass<string> Path => _path ??= new MpvPropertyReadClass<string>(this, "path", string.Empty);
        private MpvPropertyReadClass<string>? _path;

        /// <summary>
        /// The full path to the currently played media. This is different only from path in special cases. In particular, if --ytdl=yes is used, and the URL is detected by youtube-dl, then the script will set this property to the actual media URL. This property should be set only during the on_load or on_load_fail hooks, otherwise it will have no effect (or may do something implementation defined in the future). The property is reset if playback of the current media ends.
        /// </summary>
        public MpvPropertyReadClass<string> StreamOpenFileName => _streamOpenFileName ??= new MpvPropertyReadClass<string>(this, "stream-open-filename", string.Empty);
        private MpvPropertyReadClass<string>? _streamOpenFileName;

        /// <summary>
        /// If the currently played file has a title tag, use that. Otherwise, return the filename property.
        /// </summary>
        public MpvPropertyReadClass<string> MediaTitle => _mediaTitle ??= new MpvPropertyReadClass<string>(this, "media-title", string.Empty);
        private MpvPropertyReadClass<string>? _mediaTitle;

        /// <summary>
        /// Symbolic name of the file format. In some cases, this is a comma-separated list of format names, e.g. mp4 is mov,mp4,m4a,3gp,3g2,mj2 (the list may grow in the future for any format).
        /// </summary>
        public MpvPropertyReadClass<string> FileFormat => _fileFormat ??= new MpvPropertyReadClass<string>(this, "file-format", string.Empty);
        private MpvPropertyReadClass<string>? _fileFormat;

        /// <summary>
        /// Filename (full path) of the stream layer filename. (This is probably useless and is almost never different from path.)
        /// </summary>
        public MpvPropertyReadClass<string> StreamPath => _streamPath ??= new MpvPropertyReadClass<string>(this, "stream-path", string.Empty);
        private MpvPropertyReadClass<string>? _streamPath;

        /// <summary>
        /// Raw byte position in source stream. Technically, this returns the position of the most recent packet passed to a decoder.
        /// </summary>
        public MpvPropertyRead<long> StreamPos => _streamPos ??= new MpvPropertyRead<long>(this, "stream-pos", 0);
        private MpvPropertyRead<long>? _streamPos;

        /// <summary>
        /// Raw end position in bytes in source stream.
        /// </summary>
        public MpvPropertyRead<long> StreamEnd => _streamEnd ??= new MpvPropertyRead<long>(this, "stream-end", 0);
        private MpvPropertyRead<long>? _streamEnd;

        /// <summary>
        /// Duration of the current file in seconds. If the duration is unknown, the property is unavailable. Note that the file duration is not always exactly known, so this is an estimate.
        /// </summary>
        public MpvPropertyRead<double> Duration => _duration ??= new MpvPropertyRead<double>(this, "duration", null);
        private MpvPropertyRead<double>? _duration;

        /// <summary>
        /// Last A/V synchronization difference. Unavailable if audio or video is disabled.
        /// </summary>
        public MpvPropertyRead<float> AVSync => _avSync ??= new MpvPropertyRead<float>(this, "avsync", null);
        private MpvPropertyRead<float>? _avSync;

        /// <summary>
        /// Total A-V sync correction done. Unavailable if audio or video is disabled.
        /// </summary>
        public MpvPropertyRead<float> TotalAVSyncChange => _totalAVSyncChange ??= new MpvPropertyRead<float>(this, "total-avsync-change", null);
        private MpvPropertyRead<float>? _totalAVSyncChange;

        /// <summary>
        /// Video frames dropped by decoder, because video is too far behind audio (when using --framedrop=decoder). Sometimes, this may be incremented in other situations, e.g. when video packets are damaged, or the decoder doesn't follow the usual rules. Unavailable if video is disabled.
        /// </summary>
        public MpvPropertyRead<long> DecoderFrameDropCount => _decoderFrameDropCount ??= new MpvPropertyRead<long>(this, "decoder-frame-drop-count", 0);
        private MpvPropertyRead<long>? _decoderFrameDropCount;

        /// <summary>
        /// Frames dropped by VO (when using --framedrop=vo).
        /// </summary>
        public MpvPropertyRead<long> FrameDropCount => _frameDropCount ??= new MpvPropertyRead<long>(this, "frame-drop-count", 0);
        private MpvPropertyRead<long>? _frameDropCount;

        /// <summary>
        /// Number of video frames that were not timed correctly in display-sync mode for the sake of keeping A/V sync. This does not include external circumstances, such as video rendering being too slow or the graphics driver somehow skipping a vsync. It does not include rounding errors either (which can happen especially with bad source timestamps). For example, using the display-desync mode should never change this value from 0.
        /// </summary>
        public MpvPropertyRead<long> MistimedFrameCount => _mistimedFrameCount ??= new MpvPropertyRead<long>(this, "mistimed-frame-count", 0);
        private MpvPropertyRead<long>? _mistimedFrameCount;

        /// <summary>
        /// For how many vsyncs a frame is displayed on average. This is available if display-sync is active only. For 30 FPS video on a 60 Hz screen, this will be 2. This is the moving average of what actually has been scheduled, so 24 FPS on 60 Hz will never remain exactly on 2.5, but jitter depending on the last frame displayed.
        /// </summary>
        public MpvPropertyRead<float> VSyncRatio => _vSyncRatio ??= new MpvPropertyRead<float>(this, "vsync-ratio", null);
        private MpvPropertyRead<float>? _vSyncRatio;

        /// <summary>
        /// Estimated number of frames delayed due to external circumstances in display-sync mode. Note that in general, mpv has to guess that this is happening, and the guess can be inaccurate.
        /// </summary>
        public MpvPropertyRead<long> VoDelayedFrameCount => _voDelayedFrameCount ??= new MpvPropertyRead<long>(this, "vo-delayed-frame-count", 0);
        private MpvPropertyRead<long>? _voDelayedFrameCount;

        /// <summary>
        /// Position in current file (0-100). The advantage over using this instead of calculating it out of other properties is that it properly falls back to estimating the playback position from the byte position, if the file duration is not known.
        /// </summary>
        public MpvPropertyWrite<float> PercentPos => _percentPos ??= new MpvPropertyWrite<float>(this, "percent-pos", null);
        private MpvPropertyWrite<float>? _percentPos;

        /// <summary>
        /// Position in current file in seconds.
        /// </summary>
        public MpvPropertyWrite<double> TimePos => _timePos ??= new MpvPropertyWrite<double>(this, "time-pos", null);
        private MpvPropertyWrite<double>? _timePos;

        /// <summary>
        /// Remaining length of the file in seconds. Note that the file duration is not always exactly known, so this is an estimate.
        /// </summary>
        public MpvPropertyRead<double> TimeRemaining => _timeRemaining ??= new MpvPropertyRead<double>(this, "time-remaining", null);
        private MpvPropertyRead<double>? _timeRemaining;

        /// <summary>
        /// Current audio playback position in current file in seconds. Unlike time-pos, this updates more often than once per frame. For audio-only files, it is mostly equivalent to time-pos, while for video-only files this property is not available.
        /// </summary>
        public MpvPropertyRead<double> AudioPts => _audioPts ??= new MpvPropertyRead<double>(this, "audio-pts", null);
        private MpvPropertyRead<double>? _audioPts;

        /// <summary>
        /// TimeRemaining scaled by the current speed.
        /// </summary>
        public MpvPropertyRead<double> PlaytimeRemaining => _playtimeRemaining ??= new MpvPropertyRead<double>(this, "playtime-remaining", null);
        private MpvPropertyRead<double>? _playtimeRemaining;

        /// <summary>
        /// Position in current file in seconds. Unlike time-pos, the time is clamped to the range of the file. (Inaccurate file durations etc. could make it go out of range. Useful on attempts to seek outside of the file, as the seek target time is considered the current position during seeking.)
        /// </summary>
        public MpvPropertyWrite<double> PlaybackTime => _playbackTime ??= new MpvPropertyWrite<double>(this, "playback-time", null);
        private MpvPropertyWrite<double>? _playbackTime;

        /// <summary>
        /// Current chapter number. The number of the first chapter is 0.
        /// </summary>
        public MpvPropertyWrite<int> Chapter => _chapter ??= new MpvPropertyWrite<int>(this, "chapter ", null);
        private MpvPropertyWrite<int>? _chapter;

        /// <summary>
        /// Current MKV edition number. Setting this property to a different value will restart playback. The number of the first edition is 0.
        /// Before mpv 0.31.0, this showed the actual edition selected at runtime, if you didn't set the option or property manually. With mpv 0.31.0 and later, this strictly returns the user-set option or property value, and the current-edition property was added to return the runtime selected edition (this matters with --edition=auto, the default).
        /// </summary>
        public MpvPropertyWrite<int> Edition => _edition ??= new MpvPropertyWrite<int>(this, "edition", null);
        private MpvPropertyWrite<int>? _edition;

        /// <summary>
        /// Currently selected edition. This property is unavailable if no file is loaded, or the file has no editions. (Matroska files make a difference between having no editions and a single edition, which will be reflected by the property, although in practice it does not matter.)
        /// </summary>
        public MpvPropertyRead<int> CurrentEdition => _currentEdition ??= new MpvPropertyRead<int>(this, "current-edition", null);
        private MpvPropertyRead<int>? _currentEdition;

        /// <summary>
        /// Number of chapters.
        /// </summary>
        public MpvPropertyRead<int> Chapters => _chapters ??= new MpvPropertyRead<int>(this, "chapters", null);
        private MpvPropertyRead<int>? _chapters;

        /// <summary>
        /// Number of MKV editions.
        /// </summary>
        public MpvPropertyRead<int> Editions => _editions ??= new MpvPropertyRead<int>(this, "editions", null);
        private MpvPropertyRead<int>? _editions;

        /// <summary>
        /// Number of editions. If there are no editions, this can be 0 or 1 (1 if there's a useless dummy edition).
        /// </summary>
        public MpvPropertyRead<int> EditionListCount => _editionListCount ??= new MpvPropertyRead<int>(this, "edition-list/count", null);
        private MpvPropertyRead<int>? _editionListCount;

        /// <summary>
        /// Edition ID as integer. Use this to set the edition property. Currently, this is the same as the edition index.
        /// </summary>
        public MpvPropertyIndex<int> EditionListId => _editionListId ??= new MpvPropertyIndex<int>(this, "edition-list/{0}/id", null);
        private MpvPropertyIndex<int>? _editionListId;

        /// <summary>
        /// True if this is the default edition, otherwise false.
        /// </summary>
        public MpvPropertyIndexClassRead<bool?, string, int> EditionListDefault => _editionListDefault ??= new MpvPropertyIndexClassRead<bool?, string, int>(this, "edition-list/{0}/default", null,
            x => x != null ? (x == "yes") : (bool?)null);
        private MpvPropertyIndexClassRead<bool?, string, int>? _editionListDefault;

        /// <summary>
        /// Edition title as stored in the file. Not always available.
        /// </summary>
        public MpvPropertyIndexReadClass<string> EditionListTitle => _editionListTitle ??= new MpvPropertyIndexReadClass<string>(this, "edition-list/{0}/title", string.Empty);
        private MpvPropertyIndexReadClass<string>? _editionListTitle;



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
