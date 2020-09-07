using System;
using System.Collections.Generic;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Contains additional options for sending API commands.
    /// </summary>
    public class MpvCommandOptions
    {
        /// <summary>
        /// Gets the list of prefixes to add to the command.
        /// </summary>
        public IList<string> Prefixes { get; } = new List<string>();

        /// <summary>
        /// Gets or sets whether to wait for server response when sending the command.
        /// </summary>
        public bool WaitForResponse { get; set; } = true;

        /// <summary>
        /// Gets or sets the response timeout. If null, takes the default value configured for the controller.
        /// </summary>
        public int? ResponseTimeout { get; set; }

        /// <summary>
        /// Do not wait for server response and return immediately.
        /// </summary>
        public MpvCommandOptions DoNotWait()
        {
            WaitForResponse = false;
            return this;
        }

        /// <summary>
        /// Sets the timeout in milliseconds to wait for a message response. If null, the default value configured on the controller will be used.
        /// </summary>
        /// <param name="timeout">The response timeout to set.</param>
        public MpvCommandOptions SetTimeout(int? timeout = -1)
        {
            ResponseTimeout = timeout >= 0 ? timeout : -1;
            return this;
        }

        private MpvCommandOptions AddPrefix(string name)
        {
            Prefixes.Add(name);
            return this;
        }

        /// <summary>
        /// Use the default behavior for this command. This is the default for input.conf commands. Some libmpv/scripting/IPC APIs do not use this as default, but use no-osd instead.
        /// </summary>
        public MpvCommandOptions OsdAuto() => AddPrefix("osd-auto");

        /// <summary>
        /// Do not use any OSD for this command.
        /// </summary>
        public MpvCommandOptions NoOsd() => AddPrefix("no-osd");

        /// <summary>
        /// If possible, show a bar with this command. Seek commands will show the progress bar, property changing commands may show the newly set value.
        /// </summary>
        public MpvCommandOptions OsdBar() => AddPrefix("osd-bar");

        /// <summary>
        /// If possible, show an OSD message with this command. Seek command show the current playback time, property changing commands show the newly set value as text.
        /// </summary>
        public MpvCommandOptions OsdMsg() => AddPrefix("osd-msg");

        /// <summary>
        /// Combine OsdBar and OsdMsg.
        /// </summary>
        public MpvCommandOptions OsdMsgBar() => AddPrefix("osd-msg-bar");

        /// <summary>
        /// Do not expand properties in string arguments. (Like "${property-name}".) This is the default for some libmpv/scripting/IPC APIs.
        /// </summary>
        public MpvCommandOptions Raw() => AddPrefix("raw");

        /// <summary>
        /// All string arguments are expanded as described in Property Expansion. This is the default for input.conf commands.
        /// </summary>
        public MpvCommandOptions ExpandProperties() => AddPrefix("expand-properties");

        /// <summary>
        /// For some commands, keeping a key pressed doesn't run the command repeatedly. This prefix forces enabling key repeat in any case.
        /// </summary>
        public MpvCommandOptions Repeatable() => AddPrefix("repeatable");

        /// <summary>
        /// Allow asynchronous execution (if possible). Note that only a few commands will support this (usually this is explicitly documented). Some commands are asynchronous by default (or rather, their effects might manifest after completion of the command). The semantics of this flag might change in the future. Set it only if you don't rely on the effects of this command being fully realized when it returns. See Synchronous vs. Asynchronous.
        /// </summary>
        public MpvCommandOptions Async() => AddPrefix("async");

        /// <summary>
        /// Allow synchronous execution (if possible). Normally, all commands are synchronous by default, but some are asynchronous by default for compatibility with older behavior.
        /// </summary>
        public MpvCommandOptions Sync() => AddPrefix("sync");
    }
}
