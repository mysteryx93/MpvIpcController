using System;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvOption<T> : MpvPropertyWrite<T>
    {
        public MpvOption(MpvApi api, string name, CustomParser<T>? parser = null, CustomFormatter<T>? formatter = null) : base(api, name, parser, formatter)
        {
        }
    }
}
