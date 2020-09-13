using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a read-only MPV property. T must be a nullable type.
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    public class MpvPropertyRead<T> : MpvProperty<T>
    {
        public MpvPropertyRead(MpvApi api, string name) : base(api, name)
        {
        }

        /// <summary>
        /// Returns the value of the given property. The value will be sent in the data field of the replay message.
        /// </summary>
        public async Task<MpvResponse<T>?> GetAsync()
        {
            return await Api.GetPropertyAsync<T>(PropertyName).ConfigureAwait(false);
        }
    }

    ///// <summary>
    ///// Represents a read-only MPV property. TResult and TApi must be nullable types.
    ///// </summary>
    ///// <typeparam name="TResult">The return type of the property.</typeparam>
    ///// <typeparam name="TApi">The API data type before parsing.</typeparam>
    //public class MpvPropertyRead<TResult, TApi> : MpvProperty<TResult, TApi>
    //{
    //    public MpvPropertyRead(MpvApi api, string name, PropertyParser<TResult, TApi>? parser = null) : base(api, name, parser)
    //    { }

    //    /// <summary>
    //    /// Returns the value of the given property. The value will be sent in the data field of the replay message.
    //    /// </summary>
    //    public async Task<MpvResponse<TResult>?> GetAsync()
    //    {
    //        var result = await Api.GetPropertyAsync<TApi>(PropertyName).ConfigureAwait(false);
    //        return Parser(result);
    //    }
    //}
}
