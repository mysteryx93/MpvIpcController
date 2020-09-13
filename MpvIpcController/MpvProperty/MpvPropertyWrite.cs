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
        public MpvPropertyWrite(MpvApi api, string name) : base(api, name)
        {
        }

        /// <summary>
        /// Set the given property or option to the given value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public Task SetAsync(T value, MpvCommandOptions? options = null) => Api.SetPropertyAsync(PropertyName, value, options);

        /// <summary>
        /// Add the given value to the property or option. On overflow or underflow, clamp the property to the maximum. If <value> is omitted, assume 1.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public Task AddAsync(T value, MpvCommandOptions? options = null) => Api.AddAsync(PropertyName, value, options);

        /// <summary>
        /// Similar to add, but multiplies the property or option with the numeric value.
        /// </summary>
        /// <param name="value">The multiplication factor.</param>
        public Task MultiplyAsync(T value, MpvCommandOptions? options = null) => Api.MultiplyAsync(PropertyName, value, options);

        /// <summary>
        /// Cycles the given property or option. The second argument can be up or down to set the cycle direction. On overflow, set the property back to the minimum, on underflow set it to the maximum.
        /// </summary>
        /// <param name="direction">The cycling direction. By default, Up.</param>
        public Task CycleAsync(CycleDirection direction = CycleDirection.Up, MpvCommandOptions? options = null) => Api.CycleAsync(PropertyName, direction, options);

    }

    ///// <summary>
    ///// Represents a read/write MPV property.
    ///// </summary>
    ///// <typeparam name="TResult">The return type of the property.</typeparam>
    ///// <typeparam name="TApi">The API data type before parsing.</typeparam>
    //public class MpvPropertyWrite<TResult, TApi> : MpvPropertyRead<TResult, TApi>
    //{
    //    public MpvPropertyWrite(MpvApi api, string name, PropertyParser<TResult, TApi>? parser = null, PropertyFormatter<TResult, TApi>? formatter = null) : base(api, name, parser)
    //    {
    //        Formatter = formatter ?? MpvFormatters.FormatDefault<TResult, TApi>;
    //    }

    //    protected PropertyFormatter<TResult, TApi> Formatter { get; private set; }

    //    /// <summary>
    //    /// Set the given property or option to the given value.
    //    /// </summary>
    //    /// <param name="value">The value to set.</param>
    //    public Task SetAsync(TResult value, MpvCommandOptions? options = null) => Api.SetPropertyAsync(PropertyName, Formatter(value), options);

    //    /// <summary>
    //    /// Add the given value to the property or option. On overflow or underflow, clamp the property to the maximum. If <value> is omitted, assume 1.
    //    /// </summary>
    //    /// <param name="value">The value to add.</param>
    //    public Task AddAsync(TResult value, MpvCommandOptions? options = null) => Api.AddAsync(PropertyName, Formatter(value), options);

    //    /// <summary>
    //    /// Similar to add, but multiplies the property or option with the numeric value.
    //    /// </summary>
    //    /// <param name="value">The multiplication factor.</param>
    //    public Task MultiplyAsync(TResult value, MpvCommandOptions? options = null) => Api.MultiplyAsync(PropertyName, Formatter(value), options);

    //    /// <summary>
    //    /// Cycles the given property or option. The second argument can be up or down to set the cycle direction. On overflow, set the property back to the minimum, on underflow set it to the maximum.
    //    /// </summary>
    //    /// <param name="direction">The cycling direction. By default, Up.</param>
    //    public Task CycleAsync(CycleDirection direction = CycleDirection.Up, MpvCommandOptions? options = null) => Api.CycleAsync(PropertyName, direction, options);
    //}
}
