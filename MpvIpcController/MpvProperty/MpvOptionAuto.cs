using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvOptionAuto<T> : MpvOption<T>
        where T : struct
    {
        public MpvOptionAuto(MpvApi api, string name) :
            base(api, name)
        {
        }

        /// <summary>
        /// Sets the option to 'auto'.
        /// </summary>
        public Task SetAutoAsync(ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName, "auto", options);

        /// <summary>
        /// Gets whether the option is 'auto'.
        /// </summary>
        public async Task<bool> GetAutoAsync(ApiOptions? options = null)
        {
            var result = await Api.GetPropertyAsync<string?>(PropertyName, options).ConfigureAwait(false);
            return result != null && result.HasValue && result.Value() == "auto";
        }

        /// <summary>
        /// Parse value as specified type without throwing any exception on failure.
        /// </summary>
        /// <param name="value">The raw value to parse.</param>
        /// <returns>The typed parsed value.</returns>
        protected override T? ParseValue(string? value)
        {
            try
            {
                return base.ParseValue(value);
            }
            catch (FormatException)
            {
                return null;
            }
            catch (OverflowException)
            {
                return null;
            }
        }
    }

    public class MpvOptionAutoRef<T> : MpvPropertyWriteRef<T>
        where T : class
    {
        public MpvOptionAutoRef(MpvApi api, string name) :
            base(api, name)
        {
        }

        /// <summary>
        /// Sets the option to 'auto'.
        /// </summary>
        public Task SetAutoAsync(ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName, "auto", options);

        /// <summary>
        /// Gets whether the option is 'auto'.
        /// </summary>
        public async Task<bool> GetAutoAsync(ApiOptions? options = null)
        {
            var result = await Api.GetPropertyAsync<string?>(PropertyName, options).ConfigureAwait(false);
            return result != null && result.HasValue && result.Data == "auto";
        }

        /// <summary>
        /// Parse value as specified type without throwing any exception on failure.
        /// </summary>
        /// <param name="value">The raw value to parse.</param>
        /// <returns>The typed parsed value.</returns>
        protected override T? ParseValue(string? value)
        {
            try
            {
                return base.ParseValue(value);
            }
            catch (FormatException)
            {
                return null;
            }
            catch (OverflowException)
            {
                return null;
            }
        }
    }
}
