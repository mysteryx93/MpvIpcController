using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a read/write MPV property.
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    public class MpvPropertyWrite<T> : MpvPropertyRead<T>
    {
        private readonly CustomFormatter<T>? _formatter;

        public MpvPropertyWrite(MpvApi api, string name, CustomParser<T>? parser = null, CustomFormatter<T>? formatter = null) : base(api, name, parser)
        {
            _formatter = formatter;
        }

        protected object? FormatValue(T value) => _formatter != null ? _formatter(value) : value;

        /// <summary>
        /// Set the given property or option to the given value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public Task SetAsync(T value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName, FormatValue(value), options);

        /// <summary>
        /// Add the given value to the property or option. On overflow or underflow, clamp the property to the maximum. If <value> is omitted, assume 1.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public Task AddAsync(T value, ApiOptions? options = null) => Api.AddAsync(PropertyName, FormatValue(value), options);

        /// <summary>
        /// Similar to add, but multiplies the property or option with the numeric value.
        /// </summary>
        /// <param name="value">The multiplication factor.</param>
        public Task MultiplyAsync(T value, ApiOptions? options = null) => Api.MultiplyAsync(PropertyName, FormatValue(value), options);

        /// <summary>
        /// Cycles the given property or option. The second argument can be up or down to set the cycle direction. On overflow, set the property back to the minimum, on underflow set it to the maximum.
        /// </summary>
        /// <param name="direction">The cycling direction. By default, Up.</param>
        public Task CycleAsync(CycleDirection direction = CycleDirection.Up, ApiOptions? options = null) => Api.CycleAsync(PropertyName, direction, options);
    }
}
