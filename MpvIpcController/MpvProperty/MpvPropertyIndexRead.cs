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
    public class MpvPropertyIndexRead<TIndex, T> : MpvProperty<T>
    {
        public MpvPropertyIndexRead(MpvApi api, string name) : base(api, name)
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
        public async Task<MpvResponse<T>?> GetAsync(TIndex index)
        {
            return await Api.GetPropertyAsync<T>(GetPropertyIndexName(index)).ConfigureAwait(false);
        }

        /// <summary>
        /// Watches a property for changes. If the given property is changed, then an event 'property-change' will be generated.
        /// </summary>
        /// <param name="observeId">An ID that will be passed to the generated events as parameter 'id'.</param>
        public Task ObserveAsync(int observeId, TIndex index, MpvCommandOptions? options = null) =>
            Api.ObservePropertyAsync(observeId, GetPropertyIndexName(index), options);
    }

    ///// <summary>
    ///// Represents a read-only MPV indexed property. This is an exact copy of MpvPropertyIndexRead but with "where TApi : class".
    ///// </summary>
    ///// <typeparam name="TIndex">The indexer data type.</typeparam>
    ///// <typeparam name="TResult">The return type of the property.</typeparam>
    ///// <typeparam name="TApi">The API data type before parsing.</typeparam>
    //public class MpvPropertyIndexRead<TIndex, TResult, TApi> : MpvProperty<TResult, TApi>
    //{
    //    public MpvPropertyIndexRead(MpvApi api, string name, PropertyParser<TResult, TApi>? parser = null) : base(api, name, parser)
    //    {
    //    }

    //    /// <summary>
    //    /// Returns the property name after replacing {0} with specified index.
    //    /// </summary>
    //    /// <param name="index">The index to insert into the property name.</param>
    //    /// <returns>The indexed property name.</returns>
    //    public string GetPropertyIndexName(TIndex index) => PropertyName.FormatInvariant(index);

    //    /// <summary>
    //    /// Returns the value of the given property. The value will be sent in the data field of the replay message.
    //    /// </summary>
    //    /// <param name="index">The index to insert into the property name.</param>
    //    public async Task<MpvResponse<TResult>> GetAsync(TIndex index)
    //    {
    //        var result = await Api.GetPropertyAsync<TApi>(GetPropertyIndexName(index)).ConfigureAwait(false);
    //        return Parser(result);
    //    }

    //    /// <summary>
    //    /// Watches a property for changes. If the given property is changed, then an event 'property-change' will be generated.
    //    /// </summary>
    //    /// <param name="observeId">An ID that will be passed to the generated events as parameter 'id'.</param>
    //    public Task ObserveAsync(int observeId, TIndex index, MpvCommandOptions? options = null) =>
    //        Api.ObservePropertyAsync(observeId, GetPropertyIndexName(index), options);
    //}
}
