using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a comma-delimited option list of key-value pairs. ex: Val1=a,Val2=b
    /// </summary>
    public class MpvOptionDictionary : MpvOptionRef<IDictionary<string, string>>
    {
        private readonly char _separator;

        public MpvOptionDictionary(MpvApi api, string name, bool isPath = false) : base(api, name)
        {
            _separator = isPath ? System.IO.Path.PathSeparator : ',';
        }

        protected override IDictionary<string, string>? ParseValue(string? value)
        {
            var listSplit = value?.Split(_separator) ?? Array.Empty<string>();
            var result = new Dictionary<string, string>(listSplit.Length);
            foreach (var item in listSplit)
            {
                var pair = ParseKeyValue(item);
                result.Add(pair.Key, pair.Value);
            }
            return result;
        }

        protected override string? FormatValue(IDictionary<string, string>? values)
        {
            return string.Join(_separator.ToString(), values.Select(x => FormatKeyValue(x.Key, x.Value)));
        }

        private static KeyValuePair<string, string> ParseKeyValue(string data)
        {
            var dataSplit = data.Split('=');
            if (!dataSplit.Any())
            {
                throw new FormatException($"Key-value pair '{data}' is not in a valid format.");
            }
            var key = dataSplit[0];
            var value = dataSplit.Length > 1 ? dataSplit[1] : string.Empty;
            return new(key, value);
        }

        private static string FormatKeyValue(string key, string value) => $"{key}={value}";

        /// <summary>
        /// Set a list of items (using the list separator, interprets escapes).
        /// </summary>
        public Task SetAsync(string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-set", value, options);

        /// <summary>
        /// Sets a dictionary of items.
        /// </summary>
        public new async Task SetAsync(IDictionary<string, string> values, ApiOptions? options = null)
        {
            await Api.SetPropertyAsync(PropertyName + "-set", FormatValue(values), options).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the value of specified key.
        /// </summary>
        public async Task<string> GetAsync(string key, ApiOptions? options = null)
        {
            var values = await GetAsync(options).ConfigureAwait(false);
            return values[key];
        }

        /// <summary>
        /// Gets a dictionary containing all values.
        /// </summary>
        public new async Task<IDictionary<string, string>> GetAsync(ApiOptions? options = null)
        {
            var values = await Api.GetPropertyAsync(PropertyName, options).ConfigureAwait(false);
            return ParseValue(values.Data) ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Append single item (does not interpret escapes).
        /// </summary>
        public Task AppendAsync(string key, string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-append", FormatKeyValue(key, value), options);

        /// <summary>
        /// Append 1 or more items (same syntax as Set).
        /// </summary>
        public Task AddAsync(string key, string value, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-add", FormatKeyValue(key, value), options);

        /// <summary>
        /// Delete item if present (does not interpret escapes).
        /// </summary>
        public Task RemoveAsync(string key, ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName + "-remove", key, options);
    }
}
