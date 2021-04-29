using System;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvOption<T> : MpvPropertyWrite<T>
        where T : struct
    {
        public MpvOption(MpvApi api, string name) : base(api, name)
        {
        }
    }

    public class MpvOptionRef<T> : MpvPropertyWriteRef<T>
        where T : class
    {
        public MpvOptionRef(MpvApi api, string name) : base(api, name)
        {
        }
    }

    public class MpvOptionString : MpvOptionRef<string>
    {
        public MpvOptionString(MpvApi api, string name) : base(api, name)
        {
        }
    }
}
