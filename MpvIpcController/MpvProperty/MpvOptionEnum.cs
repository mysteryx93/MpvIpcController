using System;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvOptionEnum<T> : MpvPropertyWrite<T?>
        where T : struct, Enum
    {
        public MpvOptionEnum(MpvApi api, string name, CustomParser<T?>? parser = null, CustomFormatter<T?>? formatter = null) : base(api, name, parser ?? MpvFormatters.ParseEnum<T>, formatter ?? MpvFormatters.FormatEnum<T>)
        {
        }
    }
}
