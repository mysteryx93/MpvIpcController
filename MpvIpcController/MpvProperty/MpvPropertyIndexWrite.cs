using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a read/write MPV indexed property with an integer index.
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    public class MpvPropertyIndexWrite<T> : MpvPropertyIndexWrite<T, T, int>
        where T : struct
    {
        public MpvPropertyIndexWrite(MpvApi api, string name, T? defaultValue = null) : base(api, name, defaultValue)
        {
        }
    }

    /// <summary>
    /// Represents a read/write MPV indexed property.
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    /// <typeparam name="TIndex">The indexer data type.</typeparam>
    public class MpvPropertyIndexWrite<T, TIndex> : MpvPropertyIndexWrite<T, T, TIndex>
        where T : struct
    {
        public MpvPropertyIndexWrite(MpvApi api, string name, T? defaultValue = null) : base(api, name, defaultValue)
        {
        }
    }

    /// <summary>
    /// Represents a read/write MPV indexed property.
    /// </summary>
    /// <typeparam name="TResult">The return type of the property.</typeparam>
    /// <typeparam name="TApi">The API data type before parsing.</typeparam>
    /// <typeparam name="TIndex">The indexer data type.</typeparam>
    public class MpvPropertyIndexWrite<TResult, TApi, TIndex> : MpvPropertyIndexRead<TResult, TApi, TIndex>
        where TApi : struct
    {
        public MpvPropertyIndexWrite(MpvApi api, string name, TApi? defaultValue = null, PropertyParser<TResult, TApi?>? parser = null, PropertyFormatter<TResult, TApi?>? formatter = null) : base(api, name, defaultValue, parser)
        {
            Formatter = formatter ?? MpvFormatters.FormatDefault<TResult, TApi?>;
        }

        protected PropertyFormatter<TResult, TApi?> Formatter { get; private set; }

        /// <summary>
        /// Set the given property or option to the given value.
        /// </summary>
        /// <param name="index">The index to insert into the property name.</param>
        /// <param name="value">The value to set.</param>
        public Task SetAsync(TIndex index, TResult value, MpvCommandOptions? options = null) => Api.SetPropertyAsync(GetPropertyIndexName(index), Formatter(value), options);

        /// <summary>
        /// Add the given value to the property or option. On overflow or underflow, clamp the property to the maximum. If <value> is omitted, assume 1.
        /// </summary>
        /// <param name="index">The index to insert into the property name.</param>
        /// <param name="value">The value to add.</param>
        public Task AddAsync(TIndex index, TResult value, MpvCommandOptions? options = null) => Api.AddAsync(GetPropertyIndexName(index), Formatter(value), options);

        /// <summary>
        /// Similar to add, but multiplies the property or option with the numeric value.
        /// </summary>
        /// <param name="index">The index to insert into the property name.</param>
        /// <param name="value">The multiplication factor.</param>
        public Task MultiplyAsync(TIndex index, TResult value, MpvCommandOptions? options = null) => Api.MultiplyAsync(GetPropertyIndexName(index), Formatter(value), options);

        /// <summary>
        /// Cycles the given property or option. The second argument can be up or down to set the cycle direction. On overflow, set the property back to the minimum, on underflow set it to the maximum.
        /// </summary>
        /// <param name="index">The index to insert into the property name.</param>
        /// <param name="direction">The cycling direction. By default, Up.</param>
        public Task CycleAsync(TIndex index, CycleDirection direction = CycleDirection.Up, MpvCommandOptions? options = null) => Api.CycleAsync(GetPropertyIndexName(index), direction, options);
    }
}
