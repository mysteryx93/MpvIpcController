using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvOptionList : MpvOption<string?>
    {
        private readonly string _separator;

        public MpvOptionList(MpvApi api, string name, CustomParser<string?>? parser = null, CustomFormatter<string?>? formatter = null, bool isPath = false) : base(api, name, parser, formatter)
        {
            _separator = isPath ? System.IO.Path.PathSeparator.ToString() : ",";
        }

        /// <summary>
        /// Set a list of items (using the list separator, interprets escapes).
        /// </summary>
        public new Task SetAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-set", FormatValue(value), options);

        /// <summary>
        /// Set a list of items.
        /// </summary>
        public Task SetAsync(IEnumerable<string> values, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-set", FormatValue(string.Join(_separator, values)), options);

        /// <summary>
        /// Append single item (does not interpret escapes).
        /// </summary>
        public Task AppendAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-append", FormatValue(value), options);

        /// <summary>
        /// Append 1 or more items (same syntax as Set).
        /// </summary>
        public new Task AddAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-add", FormatValue(value), options);

        /// <summary>
        /// Prepend 1 or more items (same syntax as Set).
        /// </summary>
        public Task PreAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-pre", FormatValue(value), options);

        /// <summary>
        /// Clear the option (remove all items).
        /// </summary>
        public Task ClearAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-clr", FormatValue(value), options);

        /// <summary>
        /// Delete item if present (does not interpret escapes).
        /// </summary>
        public Task RemoveAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-remove", FormatValue(value), options);

        /// <summary>
        /// Append an item, or remove if if it already exists (no escapes).
        /// </summary>
        public Task ToggleAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-toggle", FormatValue(value), options);
    }
}
