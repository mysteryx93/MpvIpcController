using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Provides a base class for option types that allow extra values, such as Integer value also allowing Auto and No.
    /// </summary>
    /// <typeparam name="T">The type of regular data.</typeparam>
    public class MpvOptionWith<T> : MpvOption<T>
        where T : struct
    {
        public MpvOptionWith(MpvApi api, string name) :
            base(api, name)
        {
        }

        /// <summary>
        /// Sets the option to specified raw value.
        /// </summary>
        protected Task SetValueAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName, value, options);

        /// <summary>
        /// Gets whether the option is specified raw value.
        /// </summary>
        protected Task<bool> GetValueAsync(string value, ApiOptions? options = null) => GetValueAsync(new[] { value }, options);

        /// <summary>
        /// Gets whether the option is in specified raw values.
        /// </summary>
        protected async Task<bool> GetValueAsync(IEnumerable<string> values, ApiOptions? options = null)
        {
            var result = await Api.GetPropertyAsync<string?>(PropertyName, options).ConfigureAwait(false);
            return result != null && result.HasValue && values.Contains(result.Data);
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
