using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a read/write MPV indexed property.
    /// </summary>
    /// <typeparam name="TIndex">The indexer data type.</typeparam>
    /// <typeparam name="T">The return type of the property.</typeparam>
    public class MpvPropertyIndexWrite<TIndex, T> : MpvPropertyIndexRead<TIndex, T>
    {
        private readonly CustomFormatter<T>? _formatter;

        public MpvPropertyIndexWrite(MpvApi api, string name, CustomParser<T>? parser = null, CustomFormatter<T>? formatter = null) : base(api, name, parser)
        {
            _formatter = formatter;
        }

        private object? FormatValue(T value) => _formatter != null ? _formatter(value) : value;

        /// <summary>
        /// Set the given property or option to the given value.
        /// </summary>
        /// <param name="index">The index to insert into the property name.</param>
        /// <param name="value">The value to set.</param>
        public Task SetAsync(TIndex index, T value, ApiOptions? options = null) => Api.SetPropertyAsync(GetPropertyIndexName(index), FormatValue(value), options);

        /// <summary>
        /// Add the given value to the property or option. On overflow or underflow, clamp the property to the maximum. If <value> is omitted, assume 1.
        /// </summary>
        /// <param name="index">The index to insert into the property name.</param>
        /// <param name="value">The value to add.</param>
        public Task AddAsync(TIndex index, T value, ApiOptions? options = null) => Api.AddAsync(GetPropertyIndexName(index), FormatValue(value), options);

        /// <summary>
        /// Similar to add, but multiplies the property or option with the numeric value.
        /// </summary>
        /// <param name="index">The index to insert into the property name.</param>
        /// <param name="value">The multiplication factor.</param>
        public Task MultiplyAsync(TIndex index, T value, ApiOptions? options = null) => Api.MultiplyAsync(GetPropertyIndexName(index), FormatValue(value), options);

        /// <summary>
        /// Cycles the given property or option. The second argument can be up or down to set the cycle direction. On overflow, set the property back to the minimum, on underflow set it to the maximum.
        /// </summary>
        /// <param name="index">The index to insert into the property name.</param>
        /// <param name="direction">The cycling direction. By default, Up.</param>
        public Task CycleAsync(TIndex index, CycleDirection direction = CycleDirection.Up, ApiOptions? options = null) => Api.CycleAsync(GetPropertyIndexName(index), direction, options);
    }
}
