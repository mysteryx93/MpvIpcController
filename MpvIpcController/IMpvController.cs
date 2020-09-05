﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public interface IMpvController : IDisposable
    {
        /// <summary>
        /// Occurs when an event is received.
        /// </summary>
        event EventHandler<MpvMessageEventArgs>? EventReceived;
        /// <summary>
        /// Gets or sets the timeout in milliseconds to wait for a message response.
        /// </summary>
        int ResponseTimeout { get; set; }
        /// <summary>
        /// Gets or sets whether to keep a log of communication data.
        /// </summary>
        bool LogEnabled { get; set; }
        /// <summary>
        /// Gets a text log of communication data from both directions.
        /// </summary>
        StringBuilder? Log { get; }
        /// <summary>
        /// Sends specified message to MPV.
        /// </summary>
        /// <param name="commandName">The command to send.</param>
        /// <param name="args">Additional command parameters.</param>
        /// <returns>The server's response to the command.</returns>
        Task<object?> SendMessageAsync(string commandName, params object[] args);
    }
}
