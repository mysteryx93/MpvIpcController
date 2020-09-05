using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public interface IMpvApi : IMpvController
    {
        Task<string> GetClientName();
    }
}
