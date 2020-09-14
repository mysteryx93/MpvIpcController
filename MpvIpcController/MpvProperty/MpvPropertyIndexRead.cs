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
        private readonly CustomParser<T>? _parser;

        public MpvPropertyIndexRead(MpvApi api, string name, CustomParser<T>? parser = null) : base(api, name)
        {
            _parser = parser;
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
        public async Task<MpvResponse<T>?> GetAsync(TIndex index, ApiOptions? options)
        {
            var result = await Api.GetPropertyAsync(GetPropertyIndexName(index), options).ConfigureAwait(false);
            if (_parser != null)
            {
                return result.Copy(_parser(result));
            }
            return result.Parse<T>();
        }

        /// <summary>
        /// Watches a property for changes. If the given property is changed, then an event 'property-change' will be generated.
        /// </summary>
        /// <param name="observeId">An ID that will be passed to the generated events as parameter 'id'.</param>
        public Task ObserveAsync(int observeId, TIndex index, ApiOptions? options = null) =>
            Api.ObservePropertyAsync(observeId, GetPropertyIndexName(index), options);
    }
}
