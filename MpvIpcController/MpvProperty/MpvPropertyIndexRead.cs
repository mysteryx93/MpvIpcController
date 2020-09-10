using System;
using System.Threading.Tasks;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a read-only MPV indexed property. This is an exact copy of MpvPropertyIndexRead but with "where TApi : class".
    /// </summary>
    /// <typeparam name="TIndex">The indexer data type.</typeparam>
    /// <typeparam name="T">The return type of the property.</typeparam>
    public class MpvPropertyIndexRead<TIndex, T> : MpvPropertyIndexRead<TIndex, T, T>
    {
        public MpvPropertyIndexRead(MpvApi api, string name) : base(api, name)
        {
        }
    }

    /// <summary>
    /// Represents a read-only MPV indexed property. This is an exact copy of MpvPropertyIndexRead but with "where TApi : class".
    /// </summary>
    /// <typeparam name="TIndex">The indexer data type.</typeparam>
    /// <typeparam name="TResult">The return type of the property.</typeparam>
    /// <typeparam name="TApi">The API data type before parsing.</typeparam>
    public class MpvPropertyIndexRead<TIndex, TResult, TApi> : MpvProperty<TResult, TApi>
    {
        public MpvPropertyIndexRead(MpvApi api, string name, PropertyParser<TResult, TApi>? parser = null) : base(api, name, parser)
        {
        }

        /// <summary>
        /// Returns the property name after replacing {0} with specified index.
        /// </summary>
        /// <param name="index">The index to insert into the property name.</param>
        /// <returns>The indexed property name.</returns>
        public string GetPropertyIndexName(TIndex index) => PropertyName.FormatInvariant(index);

        /// <summary>
        /// Returns the value of the given property. The value will be sent in the data field of the replay message.
        /// </summary>
        /// <param name="index">The index to insert into the property name.</param>
        public async Task<TResult> GetAsync(TIndex index)
        {
            var result = await Api.GetPropertyAsync<TApi>(GetPropertyIndexName(index)).ConfigureAwait(false);
            return Parser(result);
        }
    }
}
