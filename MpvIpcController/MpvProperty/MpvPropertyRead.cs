using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a read-only MPV property.
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    public class MpvPropertyRead<T> : MpvPropertyRead<T?, T>
        where T : struct
    {
        public MpvPropertyRead(MpvApi api, string name, T? defaultValue = null) : base(api, name, defaultValue)
        {
        }
    }

    /// <summary>
    /// Represents a read-only MPV property.
    /// </summary>
    /// <typeparam name="TResult">The return type of the property.</typeparam>
    /// <typeparam name="TApi">The API data type before parsing.</typeparam>
    public class MpvPropertyRead<TResult, TApi> : MpvProperty<TResult, TApi>
        where TApi : struct
    {
        public MpvPropertyRead(MpvApi api, string name, TApi? defaultValue = null, PropertyParser<TResult, TApi?>? parser = null) : base(api, name, defaultValue, parser)
        { }

        /// <summary>
        /// Returns the value of the given property. The value will be sent in the data field of the replay message.
        /// </summary>
        public async Task<TResult> GetAsync()
        {
            var result = await Api.GetPropertyAsync<TApi>(PropertyName).ConfigureAwait(false) ?? DefaultValue;
            return Parser(result);
        }
    }
}
