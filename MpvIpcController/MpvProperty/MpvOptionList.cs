using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a comma-delimited option list of string values.
    /// </summary>
    public class MpvOptionList : MpvOptionRef<IEnumerable<string>>
    {
        // Note: API doesn't support escaping, so there's no way of interpreting values containing a separator.
        // As a reliable work-around, all APIs interpreting separators are discarted. The implemented methods work reliably with any values.

        public MpvOptionList(MpvApi api, string name) : base(api, name)
        {
            // _separator = isPath ? System.IO.Path.PathSeparator : ',';
        }

        /// <summary>
        /// Get the list of items.
        /// </summary>
        public new async Task<IEnumerable<string>> GetAsync(ApiOptions? options = null)
        {
            var query = await Api.GetPropertyAsync(PropertyName, options).ConfigureAwait(false);
            return ParseValue(query.Data) ?? Array.Empty<string>();
        }

        /// <summary>
        /// Set a list of items (using the list separator, interprets escapes).
        /// </summary>
        public Task SetAsync(string value, ApiOptions? options = null) => SetAsync(new[] { value }, options);

        /// <summary>
        /// Set a list of items.
        /// </summary>
        public override async Task SetAsync(IEnumerable<string> values, ApiOptions? options = null)
        {
            // For some properties, SetProperty calls Append instead of Set, so we clear first for consistency.
            await ClearAsync(options).ConfigureAwait(false);
            await AddAsync(values, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Append single item.
        /// </summary>
        public async Task AddAsync(string value, ApiOptions? options = null)
        {
            value.CheckNotNullOrEmpty(nameof(value));

            await Api.ChangeListAsync(PropertyName, ListOptionOperation.Append, value, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds a list of items to the list.
        /// </summary>
        /// <param name="values">The list of items to add.</param>
        /// <returns></returns>
        public override async Task AddAsync(IEnumerable<string> values, ApiOptions? options = null)
        {
            foreach (var item in values)
            {
                if (item.HasValue())
                {
                    await AddAsync(item, options).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Clear the option (remove all items).
        /// </summary>
        public Task ClearAsync(ApiOptions? options = null) => Api.ChangeListAsync(PropertyName, ListOptionOperation.Clr, string.Empty, options);

        /// <summary>
        /// Delete item if present (does not interpret escapes).
        /// </summary>
        public async Task RemoveAsync(string value, ApiOptions? options = null)
        {
            value.CheckNotNullOrEmpty(nameof(value));

            await Api.ChangeListAsync(PropertyName, ListOptionOperation.Remove, value, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Append an item, or remove if if it already exists (no escapes).
        /// </summary>
        public Task ToggleAsync(string value, ApiOptions? options = null) => Api.ChangeListAsync(PropertyName, ListOptionOperation.Toggle, value, options);
    }
}
