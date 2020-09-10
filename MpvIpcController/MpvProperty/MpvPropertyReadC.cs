using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a read-only MPV property. This is an exact copy of MpvPropertyRead but with "where TApi : class".
    /// </summary>
    /// <typeparam name="TResult">The return type of the property.</typeparam>
    public class MpvPropertyReadC<T> : MpvPropertyReadC<T?, T>
        where T : class
    {
        public MpvPropertyReadC(MpvApi api, string name, T? defaultValue = null) : base(api, name, defaultValue)
        { }
    }

    /// <summary>
    /// Represents a read-only MPV property. This is an exact copy of MpvPropertyRead but with "where TApi : class".
    /// </summary>
    /// <typeparam name="TResult">The return type of the property.</typeparam>
    /// <typeparam name="TApi">The API data type before parsing.</typeparam>
    public class MpvPropertyReadC<TResult, TApi> : MpvPropertyC<TResult, TApi>
        where TApi : class
    {
        public MpvPropertyReadC(MpvApi api, string name, TApi? defaultValue = null, PropertyParser<TResult, TApi?>? parser = null) : base(api, name, defaultValue, parser)
        { }

        /// <summary>
        /// Returns the value of the given property. The value will be sent in the data field of the replay message.
        /// </summary>
        public async Task<TResult> GetAsync()
        {
            var result = await Api.GetPropertyClassAsync<TApi>(PropertyName).ConfigureAwait(false) ?? DefaultValue;
            return Parser(result);
        }
    }
}
