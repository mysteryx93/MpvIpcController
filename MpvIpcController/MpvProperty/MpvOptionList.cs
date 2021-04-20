using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a comma-delimited option list of string values.
    /// </summary>
    public class MpvOptionList : MpvOptionRef<IEnumerable<string>>
    {
        private readonly char _separator;

        public MpvOptionList(MpvApi api, string name, bool isPath = false) : base(api, name)
        {
            _separator = isPath ? System.IO.Path.PathSeparator : ',';
        }

        protected override IEnumerable<string> ParseValue(string? value)
        {
            return value?.Split(_separator) ?? Array.Empty<string>();
        }

        protected override string? FormatValue(IEnumerable<string>? values)
        {
            return string.Join(_separator.ToString(), values);
        }

        /// <summary>
        /// Get the list of items.
        /// </summary>
        public new async Task<IEnumerable<string>> GetAsync(ApiOptions? options = null)
        {
            var query = await Api.GetPropertyAsync(PropertyName, options).ConfigureAwait(false);
            return ParseValue(query.Data);
        }

        /// <summary>
        /// Set a list of items (using the list separator, interprets escapes).
        /// </summary>
        public Task SetAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-set", value, options);

        /// <summary>
        /// Set a list of items.
        /// </summary>
        public new Task SetAsync(IEnumerable<string> values, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-set", FormatValue(values), options);

        /// <summary>
        /// Append single item (does not interpret escapes).
        /// </summary>
        public Task AppendAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-append", value, options);

        /// <summary>
        /// Append 1 or more items (same syntax as Set).
        /// </summary>
        public Task AddAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-add", value, options);

        /// <summary>
        /// Prepend 1 or more items (same syntax as Set).
        /// </summary>
        public Task PreAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-pre", value, options);

        /// <summary>
        /// Clear the option (remove all items).
        /// </summary>
        public Task ClearAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-clr", value, options);

        /// <summary>
        /// Delete item if present (does not interpret escapes).
        /// </summary>
        public Task RemoveAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-remove", value, options);

        /// <summary>
        /// Append an item, or remove if if it already exists (no escapes).
        /// </summary>
        public Task ToggleAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-toggle", value, options);
    }
}
