using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

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
            LogEnabled = true;
        }

        public async Task<string> GetClientName()
        {
            var response = await SendMessageAsync("client_name").ConfigureAwait(false);
            return response?.ToString() ?? string.Empty;
        }
    }
}
