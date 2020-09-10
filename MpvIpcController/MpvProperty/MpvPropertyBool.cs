using System;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a boolean read-only API property.
    /// </summary>
    public class MpvPropertyBoolRead : MpvPropertyReadC<bool?, string>
    {
        public MpvPropertyBoolRead(MpvApi api, string name, string? defaultValue = null) : base(api, name, defaultValue, MpvFormatters.ParseBool)
        {
        }
    }

    /// <summary>
    /// Represents a boolean read-write API property.
    /// </summary>
    public class MpvPropertyBoolWrite : MpvPropertyWriteC<bool?, string>
    {
        public MpvPropertyBoolWrite(MpvApi api, string name, string? defaultValue = null) : base(api, name, defaultValue, MpvFormatters.ParseBool, MpvFormatters.FormatBool)
        {
        }
    }

    /// <summary>
    /// Represents a boolean indexed read-only API property.
    /// </summary>
    /// <typeparam name="TIndex">The indexer data type.</typeparam>
    public class MpvPropertyIndexBoolRead<TIndex> : MpvPropertyIndexReadC<bool?, string, TIndex>
    {
        public MpvPropertyIndexBoolRead(MpvApi api, string name, string? defaultValue = null) : base(api, name, defaultValue, MpvFormatters.ParseBool)
        {
        }
    }

    /// <summary>
    /// Represents a boolean indexed read-write API property.
    /// </summary>
    /// <typeparam name="TIndex">The indexer data type.</typeparam>
    public class MpvPropertyIndexBoolWrite<TIndex> : MpvPropertyIndexWriteC<bool?, string, TIndex>
    {
        public MpvPropertyIndexBoolWrite(MpvApi api, string name, string? defaultValue = null) : base(api, name, defaultValue, MpvFormatters.ParseBool)
        {
        }
    }
}
