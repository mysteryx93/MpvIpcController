using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a read/write MPV property.
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    public class MpvPropertyWrite<T> : MpvPropertyRead<T>
        where T : struct
    {
        public MpvPropertyWrite(MpvApi api, string name) : base(api, name)
        {
        }

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
        public Task MultiplyAsync(double value, ApiOptions? options = null) => Api.MultiplyAsync(PropertyName, value, options);

        /// <summary>
        /// Cycles the given property or option. The second argument can be up or down to set the cycle direction. On overflow, set the property back to the minimum, on underflow set it to the maximum.
        /// </summary>
        /// <param name="direction">The cycling direction. By default, Up.</param>
        public Task CycleAsync(CycleDirection direction = CycleDirection.Up, ApiOptions? options = null) => Api.CycleAsync(PropertyName, direction, options);
    }

    /// <summary>
    /// Represents a read/write MPV property.
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    public class MpvPropertyWriteRef<T> : MpvPropertyReadRef<T>
        where T : class
    {
        public MpvPropertyWriteRef(MpvApi api, string name) : base(api, name)
        {
        }

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

    /// <summary>
    /// Represents a read/write MPV property of type String.
    /// </summary>
    public class MpvPropertyWriteString : MpvPropertyWriteRef<string>
    {
        public MpvPropertyWriteString(MpvApi api, string name) : base(api, name)
        {
        }
    }
}
