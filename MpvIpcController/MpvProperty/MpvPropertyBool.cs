//using System;

//namespace HanumanInstitute.MpvIpcController
//{
//    /// <summary>
//    /// Represents a boolean read-only API property.
//    /// </summary>
//    public class MpvPropertyBoolRead : MpvPropertyRead<bool?, string?>
//    {
//        public MpvPropertyBoolRead(MpvApi api, string name) : base(api, name, MpvFormatters.ParseBool)
//        {
//        }
//    }

//    /// <summary>
//    /// Represents a boolean read-write API property.
//    /// </summary>
//    public class MpvPropertyBoolWrite : MpvPropertyWrite<bool?, string?>
//    {
//        public MpvPropertyBoolWrite(MpvApi api, string name) : base(api, name, MpvFormatters.ParseBool, MpvFormatters.FormatBool)
//        {
//        }
//    }

//    /// <summary>
//    /// Represents a boolean indexed read-only API property.
//    /// </summary>
//    /// <typeparam name="TIndex">The indexer data type.</typeparam>
//    public class MpvPropertyIndexBoolRead<TIndex> : MpvPropertyIndexRead<TIndex, bool?, string?>
//    {
//        public MpvPropertyIndexBoolRead(MpvApi api, string name) : base(api, name, MpvFormatters.ParseBool)
//        {
//        }
//    }

//    /// <summary>
//    /// Represents a boolean indexed read-write API property.
//    /// </summary>
//    /// <typeparam name="TIndex">The indexer data type.</typeparam>
//    public class MpvPropertyIndexBoolWrite<TIndex> : MpvPropertyIndexWrite<TIndex, bool?, string?>
//    {
//        public MpvPropertyIndexBoolWrite(MpvApi api, string name) : base(api, name, MpvFormatters.ParseBool)
//        {
//        }
//    }
//}
