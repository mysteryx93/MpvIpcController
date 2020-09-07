using System;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Contains information about an event message received from MPV.
    /// </summary>
    public class MpvMessageEventArgs : EventArgs
    {
        public MpvMessageEventArgs(MpvEvent obj)
        {
            Event = obj;
        }

        public MpvEvent Event { get; set; }
    }
}
