using System;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    public partial class MpvApi
    {
        /// <summary>
        /// Happens right before a new file is loaded. When you receive this, the player is loading the file (or possibly already done with it).
        /// </summary>
        public event EventHandler? StartFile;
        /// <summary>
        /// Happens after a file was unloaded. Typically, the player will load the next file right away, or quit if this was the last file.
        /// </summary>
        public event EventHandler<EndFileEventArgs>? EndFile;
        /// <summary>
        /// Happens after a file was loaded and begins playback.
        /// </summary>
        public event EventHandler? FileLoaded;
        /// <summary>
        /// Happens on seeking. (This might include cases when the player seeks internally, even without user interaction. This includes e.g. segment changes when playing ordered chapters Matroska files.)
        /// </summary>
        public event EventHandler? Seek;
        /// <summary>
        /// Start of playback after seek or after file was loaded.
        /// </summary>
        public event EventHandler? PlaybackRestart;
        /// <summary>
        /// Idle mode is entered. This happens when playback ended, and the player was started with --idle or --force-window. This mode is implicitly ended when the start-file or shutdown events happen.
        /// </summary>
        public event EventHandler? Idle;
        /// <summary>
        /// Called after a video frame was displayed. This is a hack, and you should avoid using it. Use timers instead and maybe watch pausing/unpausing events to avoid wasting CPU when the player is paused.
        /// </summary>
        public event EventHandler? Tick;
        /// <summary>
        /// Sent when the player quits, and the script should terminate. Normally handled automatically. See Details on the script initialization and lifecycle.
        /// </summary>
        public event EventHandler? Shutdown;
        /// <summary>
        /// Receives messages enabled with mp.enable_messages.
        /// </summary>
        public event EventHandler<LogMessageEventArgs>? LogMessage;
        /// <summary>
        /// Happens on video output or filter reconfig.
        /// </summary>
        public event EventHandler? VideoReconfig;
        /// <summary>
        /// Happens on audio output or filter reconfig.
        /// </summary>
        public event EventHandler? AudioReconfig;
        /// <summary>
        /// Happens when an observed property is changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs>? PropertyChanged;

        /// <summary>
        /// Occurs when an event is received from MPV.
        /// </summary>
        private void Mpv_EventReceived(object sender, MpvMessageEventArgs e)
        {
            if (e.EventName == "start-file")
            {
                StartFile?.Invoke(this, new EventArgs());
            }
            else if (e.EventName == "end-file" && EndFile != null)
            {
                var args = new EndFileEventArgs();
                if (Enum.TryParse(e.Data["reason"], true, out EndReason reason))
                {
                    args.Reason = reason;
                }
                else
                {
                    args.Reason = EndReason.Unknown;
                }
                EndFile?.Invoke(this, args);
            }
            else if (e.EventName == "file-loaded")
            {
                FileLoaded?.Invoke(this, new EventArgs());
            }
            else if (e.EventName == "seek")
            {
                Seek?.Invoke(this, new EventArgs());
            }
            else if (e.EventName == "playback-restart")
            {
                PlaybackRestart?.Invoke(this, new EventArgs());
            }
            else if (e.EventName == "idle")
            {
                Idle?.Invoke(this, new EventArgs());
            }
            else if (e.EventName == "tick")
            {
                Tick?.Invoke(this, new EventArgs());
            }
            else if (e.EventName == "shutdown")
            {
                Shutdown?.Invoke(this, new EventArgs());
            }
            else if (e.EventName == "log-message" && LogMessage != null)
            {
                var args = new LogMessageEventArgs
                {
                    Prefix = e.Data["prefix"] ?? string.Empty,
                    Level = FlagExtensions.ParseMpvFlag<LogLevel>(e.Data["level"]) ?? LogLevel.No,
                    Text = e.Data["text"] ?? string.Empty
                };
                LogMessage?.Invoke(this, args);
            }
            else if (e.EventName == "video-reconfig")
            {
                VideoReconfig?.Invoke(this, new EventArgs());
            }
            else if (e.EventName == "audio-reconfig")
            {
                AudioReconfig?.Invoke(this, new EventArgs());
            }
            else if (e.EventName == "property-change" && PropertyChanged != null)
            {
                var args = new PropertyChangedEventArgs()
                {
                    Id = e.Data["id"].Parse<int>() ?? 0,
                    Data = e.Data["data"] ?? string.Empty,
                    Name = e.Data["name"] ?? string.Empty
                };
                PropertyChanged?.Invoke(this, args);
            }
        }
    }
}
