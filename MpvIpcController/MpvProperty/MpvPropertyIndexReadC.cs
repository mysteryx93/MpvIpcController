using System;
using System.Threading.Tasks;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a read-only MPV indexed property with an integer index. This is an exact copy of MpvPropertyIndexRead but with "where TApi : class".
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    public class MpvPropertyIndexReadC<T> : MpvPropertyIndexReadC<T, T, int>
        where T : class
    {
        public MpvPropertyIndexReadC(MpvApi api, string name, T? defaultValue = null) : base(api, name, defaultValue)
        {
        }
    }

    /// <summary>
    /// Represents a read-only MPV indexed property. This is an exact copy of MpvPropertyIndexRead but with "where TApi : class".
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    /// <typeparam name="TIndex">The indexer data type.</typeparam>
    public class MpvPropertyIndexReadC<T, TIndex> : MpvPropertyIndexReadC<T, T, TIndex>
        where T : class
    {
        public MpvPropertyIndexReadC(MpvApi api, string name, T? defaultValue = null) : base(api, name, defaultValue)
        {
        }
    }

    /// <summary>
    /// Represents a read-only MPV indexed property. This is an exact copy of MpvPropertyIndexRead but with "where TApi : class".
    /// </summary>
    /// <typeparam name="TResult">The return type of the property.</typeparam>
    /// <typeparam name="TApi">The API data type before parsing.</typeparam>
    /// <typeparam name="TIndex">The indexer data type.</typeparam>
    public class MpvPropertyIndexReadC<TResult, TApi, TIndex> : MpvPropertyC<TResult, TApi>
        where TApi : class
    {
        public MpvPropertyIndexReadC(MpvApi api, string name, TApi? defaultValue = null, PropertyParser<TResult, TApi?>? parser = null) : base(api, name, defaultValue, parser)
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
            var result = await Api.GetPropertyClassAsync<TApi>(GetPropertyIndexName(index)).ConfigureAwait(false) ?? DefaultValue;
            return Parser(result);
        }
    }
}
